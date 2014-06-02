using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using Crichton.Representors;
using Crichton.WebApi;
using Crichton.WebApi.Extensions;
using WebApiSample.Models;

namespace WebApiSample.BuilderDescriptors
{
    public class FriendModelEnumerableBuilderDescriptor : IBuilderDescriptor
    {
        public IRepresentorBuilder BuildForType(Type type, object value, HttpRequestContext context)
        {
            var builder = new RepresentorBuilder();

            var models = (IEnumerable<FriendModel>)value;

            builder.SetCollection(models, d => context.Url.Route("DefaultApi", new { controller = "friends", id = d.Id }));
            builder.SetSelfLinkToCurrentUrl(context);

            return builder;
        }

        public bool SupportsType(Type type)
        {
            return type == typeof (IEnumerable<FriendModel>);
        }
    }
}