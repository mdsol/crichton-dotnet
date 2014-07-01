using System;
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
            if (rel == null) { throw new ArgumentNullException("rel"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToTransitionQueryStep(rel));
            return query;
        }

        public static IHypermediaQuery FollowWithData(this IHypermediaQuery hypermediaQuery, string rel, object data)
        {
            if (rel == null) { throw new ArgumentNullException("rel"); }
            if (data == null) { throw new ArgumentNullException("data"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new PostToTransitionQueryStep(rel, data));
            return query;
        }

        public static IHypermediaQuery WithUrl(this IHypermediaQuery hypermediaQuery, string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRelativeUrlQueryStep(url));
            return query;
        }

        public static IHypermediaQuery WithRepresentor(this IHypermediaQuery hypermediaQuery,
            CrichtonRepresentor representor)
        {
            if (representor == null) { throw new ArgumentNullException("representor"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }
    }
}
