﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    //talk controller
    [RoutePrefix("api/camps/{monikar}/talks")]
    public class TalksController : ApiController
    {
        private ICampRepository _repositary;
        private IMapper _mapper;
        public TalksController(ICampRepository repositary, IMapper mapper)
        {
            _repositary = repositary;
            _mapper = mapper;
        }

        [Route()]
        public async Task<IHttpActionResult> Get(string monikar, bool includeSpeakers = false)
        {
            try
            {
                var results = await _repositary.GetTalksByMonikerAsync(monikar, includeSpeakers);

                return Ok(_mapper.Map<IEnumerable<TalkModel>>(results));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("{id:int}", Name = "GetTalk")]
        public async Task<IHttpActionResult> Get(string monikar, int id, bool includeSpeakers = false)
        {
            try
            {
                var results = await _repositary.GetTalkByMonikerAsync(monikar, id, includeSpeakers);

                return Ok(_mapper.Map<TalkModel>(results));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route()]
        public async Task<IHttpActionResult> Post(string monikar, TalkModel talkModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var camp = await _repositary.GetCampAsync(monikar);
                    if (camp != null)
                    {
                        var talk = _mapper.Map<Talk>(talkModel);
                        talk.Camp = camp;

                        _repositary.AddTalk(talk);

                        if (await _repositary.SaveChangesAsync())
                        {
                            return CreatedAtRoute("GetTalk", new
                            {
                                monikar = monikar,
                                id = talk.TalkId
                            },
                            _mapper.Map<TalkModel>(talk));
                        }
                    }
                }
                
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            }
            return BadRequest(ModelState);
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Put(string monikar, int talkId, TalkModel model)
        {
            try
            {
                if(ModelState.IsValid)  {
                    var talk = await _repositary.GetTalkByMonikerAsync(monikar, talkId, true);
                    if (talk == null) return NotFound();

                   _mapper.Map(model, talk);

                    if (talk.Speaker.SpeakerId != model.Speaker.SpeakerId)
                    {
                        var speaker = await _repositary.GetSpeakerAsync(model.Speaker.SpeakerId);
                        
                        if (speaker != null) talk.Speaker = speaker;
                    }

                    if (await _repositary.SaveChangesAsync())
                    {
                        return Ok(_mapper.Map<TalkModel>(talk));
                    }
                }
            }
            catch(Exception ex)
            {
               return  InternalServerError(ex);
            }
            return BadRequest();
        }

        [Route("{talkId:int}")]
        public async Task<IHttpActionResult> Delete(string monikar, int talkId)
        {
            try
            {
                var talk = await _repositary.GetTalkByMonikerAsync(monikar, talkId);
                if (talk == null) return NotFound();

                _repositary.DeleteTalk(talk);

                if(await _repositary.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }
}