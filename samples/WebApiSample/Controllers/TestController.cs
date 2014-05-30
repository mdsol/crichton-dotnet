using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crichton.Representors;
using WebApiSample.Models;

namespace WebApiSample.Controllers
{
    public class TestController : ApiController
    {
        private static readonly IReadOnlyList<ExampleModel> Data = 
            new List<ExampleModel> { new ExampleModel() { Id = 1, Name = "Chad" }, new ExampleModel() { Id = 2, Name = "Jordi" } };

        // GET api/test
        public IRepresentorBuilder Get()
        {
            var builder = new RepresentorBuilder();

            builder.SetCollection(Data, d => "api/test/" + d.Id);
            builder.SetSelfLink("api/test");
            builder.AddTransition("friends","api/friends");

            return builder;
        }

        // GET api/test/5
        public IRepresentorBuilder Get(int id)
        {
            var data = Data.Single(d => d.Id == id);

            var builder = new RepresentorBuilder();
            builder.SetAttributesFromObject(data);
            builder.SetSelfLink("api/test/" + id);

            return builder;
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
