using System.Web.Http.Controllers;
using Crichton.Representors;

namespace Crichton.WebApi.Extensions
{
    public static class RepresentorBuilderWebApiExtensions
    {
        public static void SetSelfLinkToCurrentUrl(this IRepresentorBuilder builder, HttpRequestContext requestContext)
        {
            builder.SetSelfLink(requestContext.Url.Request.RequestUri.PathAndQuery);
        }

        public static void AddTranstionToRoute(this IRepresentorBuilder builder, HttpRequestContext requestContext, string rel, string routeName, object routeValues)
        {
            builder.AddTransition(rel, requestContext.Url.Route(routeName, routeValues));
        }
    }
}
