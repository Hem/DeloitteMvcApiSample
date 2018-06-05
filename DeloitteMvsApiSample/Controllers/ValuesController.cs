using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace DeloitteMvcApiSample.Controllers
{
    [Authorize]
    public class ValuesController : ApiController
    {


        // GET: api/Values
        public IEnumerable<string> Get()
        {
            var principal = ClaimsPrincipal.Current;
            var name = principal.Identity.Name;
            // var hash = principal.Identity.GetHashCode().ToString();
            var issuedAt = principal.FindFirst("iat")?.Value;

            var cacheToken = $"{name}-{issuedAt}";

            return new string[] { name, issuedAt, cacheToken };
        }

        // GET: api/Values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Values
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Values/5
        public void Delete(int id)
        {
        }
    }
}
