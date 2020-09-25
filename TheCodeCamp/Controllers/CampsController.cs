using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;

namespace TheCodeCamp.Controllers
{
    public class CampsController : ApiController
    {
        private readonly ICampRepository _repositary;

        public CampsController(ICampRepository repositary)
        {
            _repositary = repositary;
        } 
        // GET api/<controller>
        public async Task<IHttpActionResult> Get()
        {
            try { 
            var result = await _repositary.GetAllCampsAsync();
            return Ok( result);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}