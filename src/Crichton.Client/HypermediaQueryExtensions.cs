using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    public static class HypermediaQueryExtensions
    {
        public static IHypermediaQuery FollowSelf(this IHypermediaQuery hypermediaQuery)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToSelfLinkQueryStep());
            return query;
        }

        public static IHypermediaQuery Follow(this IHypermediaQuery hypermediaQuery, string rel)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToTransitionQueryStep(rel));
            return query;
        }

        public static IHypermediaQuery FollowWithData(this IHypermediaQuery hypermediaQuery, string rel, object data)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new PostToTransitionQueryStep(rel, data));
            return query;
        }

        public static IHypermediaQuery WithUrl(this IHypermediaQuery hypermediaQuery, string url)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRelativeUrlQueryStep(url));
            return query;
        }

        public static IHypermediaQuery WithRepresentor(this IHypermediaQuery hypermediaQuery,
            CrichtonRepresentor representor)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }
    }
}
