using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiSample.Models;

namespace WebApiSample.Controllers
{
    public class TestController : ApiController
    {
        private static readonly IReadOnlyList<ExampleModel> Data = 
            new List<ExampleModel> { new ExampleModel() { Id = 1, Name = "Chad" }, new ExampleModel() { Id = 2, Name = "Jordi" } };

        // GET api/test
        public IEnumerable<ExampleModel> Get()
        {
            return Data;
        }

        // GET api/test/5
        public ExampleModel Get(int id)
        {
            return Data.Single(d => d.Id == id);
        }

        // POST api/test
        public void Post([FromBody]string value)
        {
        }

        // PUT api/test/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/test/5
        public void Delete(int id)
        {
        }
    }
}
