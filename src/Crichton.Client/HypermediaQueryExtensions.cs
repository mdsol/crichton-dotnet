using System;
using System.Diagnostics.Contracts;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    public static class HypermediaQueryExtensions
    {
        public static IHypermediaQuery FollowSelf(this IHypermediaQuery hypermediaQuery)
        {
            Contract.Requires<ArgumentNullException>(hypermediaQuery != null, "hypermediaQuery must not be null");

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToSelfLinkQueryStep());
            return query;
        }

        public static IHypermediaQuery Follow(this IHypermediaQuery hypermediaQuery, string rel)
        {
            Contract.Requires<ArgumentNullException>(hypermediaQuery != null, "hypermediaQuery must not be null");
            Contract.Requires<ArgumentNullException>(rel != null, "rel must not be null");

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToTransitionQueryStep(rel));
            return query;
        }

        public static IHypermediaQuery FollowWithData(this IHypermediaQuery hypermediaQuery, string rel, object data)
        {
            Contract.Requires<ArgumentNullException>(hypermediaQuery != null, "hypermediaQuery must not be null");
            Contract.Requires<ArgumentNullException>(rel != null, "rel must not be null");
            Contract.Requires<ArgumentNullException>(data != null, "data must not be null");

            var query = hypermediaQuery.Clone();
            query.AddStep(new PostToTransitionQueryStep(rel, data));
            return query;
        }

        public static IHypermediaQuery WithUrl(this IHypermediaQuery hypermediaQuery, string url)
        {
            Contract.Requires<ArgumentNullException>(hypermediaQuery != null, "hypermediaQuery must not be null");
            Contract.Requires<ArgumentNullException>(url != null, "url must not be null"); 

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRelativeUrlQueryStep(url));
            return query;
        }

        public static IHypermediaQuery WithRepresentor(this IHypermediaQuery hypermediaQuery,
            CrichtonRepresentor representor)
        {
            Contract.Requires<ArgumentNullException>(hypermediaQuery != null, "hypermediaQuery must not be null");
            Contract.Requires<ArgumentNullException>(representor != null, "representor must not be null"); 

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }
    }
}
