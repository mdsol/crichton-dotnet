using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crichton.Representors;
using Crichton.WebApi;
using Crichton.WebApi.Extensions;
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
            var builder = new RepresentorBuilder(); // should use DI to inject an instance of this in.

            builder.SetCollection(Data, d => Url.Route("DefaultApi", new { controller = "test", id = d.Id }));
            builder.SetSelfLinkToCurrentUrl(RequestContext);
            builder.AddTranstionToRoute(RequestContext, "friends", "DefaultApi", new { controller = "friends" });

            return builder;
        }

        // GET api/test/5
        public IRepresentorBuilder Get(int id)
        {
            var data = Data.Single(d => d.Id == id);

            var builder = new RepresentorBuilder();
            builder.SetAttributesFromObject(data);
            builder.SetSelfLinkToCurrentUrl(RequestContext);

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
