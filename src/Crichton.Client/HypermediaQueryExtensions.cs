using System;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    /// <summary>
    /// HypermediaQueryExtensions
    /// </summary>
    public static class HypermediaQueryExtensions
    {
        /// <summary>
        /// Returns a new query being added a NavigateToSelfLinkQueryStep.
        /// </summary>
        /// <param name="hypermediaQuery">this IHypermediaQuery</param>
        /// <returns>the query</returns>
        public static IHypermediaQuery FollowSelf(this IHypermediaQuery hypermediaQuery)
        {
            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToSelfLinkQueryStep());
            return query;
        }

        /// <summary>
        /// Returns a new query being added a NavigateToTransitionQueryStep.
        /// </summary>
        /// <param name="hypermediaQuery">this IHypermediaQuery</param>
        /// <param name="rel">the rel</param>
        /// <returns>the query</returns>
        public static IHypermediaQuery Follow(this IHypermediaQuery hypermediaQuery, string rel)
        {
            if (rel == null) { throw new ArgumentNullException("rel"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToTransitionQueryStep(rel));
            return query;
        }

        /// <summary>
        /// Returns a new query being added a PostToTransitionQueryStep.
        /// </summary>
        /// <param name="hypermediaQuery">this IHypermediaQuery</param>
        /// <param name="rel">the rel</param>
        /// <param name="data">the data</param>
        /// <returns>the query</returns>
        public static IHypermediaQuery FollowWithData(this IHypermediaQuery hypermediaQuery, string rel, object data)
        {
            if (rel == null) { throw new ArgumentNullException("rel"); }
            if (data == null) { throw new ArgumentNullException("data"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new PostToTransitionQueryStep(rel, data));
            return query;
        }

        /// <summary>
        /// Returns a new query being added a NavigateToRelativeUrlQueryStep.
        /// </summary>
        /// <param name="hypermediaQuery">this IHypermediaQuery</param>
        /// <param name="url">the url</param>
        /// <returns>the query</returns>
        public static IHypermediaQuery WithUrl(this IHypermediaQuery hypermediaQuery, string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }

            var query = hypermediaQuery.Clone();
            query.AddStep(new NavigateToRelativeUrlQueryStep(url));
            return query;
        }

        /// <summary>
        /// Returns a new query being added a NavigateToRepresentorQueryStep.
        /// </summary>
        /// <param name="hypermediaQuery">this IHypermediaQuery</param>
        /// <param name="representor">the representor</param>
        /// <returns>the query</returns>
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
