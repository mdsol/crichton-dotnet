using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Crichton.Representors;
using Crichton.WebApi;
using Crichton.WebApi.Extensions;
using WebApiSample.Models;

namespace WebApiSample.BuilderDescriptors
{
    public class FriendModelBuilderDescriptor : IBuilderDescriptor
    {
        public IRepresentorBuilder BuildForType(Type type, object value, HttpRequestContext context)
        {
            var builder = new RepresentorBuilder();
            var model = (FriendModel)value;
            
            builder.SetAttributesFromObject(model);
            builder.SetSelfLinkToCurrentUrl(context);

            return builder;
        }

        public bool SupportsType(Type type)
        {
            return type == typeof (FriendModel);
        }
    }
}