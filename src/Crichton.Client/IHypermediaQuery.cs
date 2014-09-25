using System.Collections.Generic;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    /// <summary>
    /// IHypermediaQuery interface
    /// </summary>
    public interface IHypermediaQuery
    {
        /// <summary>
        /// Gets the query steps
        /// </summary>
        IList<IQueryStep> Steps { get; }

        /// <summary>
        /// Adds a query step
        /// </summary>
        /// <param name="step"></param>
        void AddStep(IQueryStep step);

        /// <summary>
        /// Executes the query
        /// </summary>
        /// <param name="requestHandler">the requestHandler</param>
        /// <returns>task including a CrichtonRepresentor</returns>
        Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestHandler requestHandler);

        /// <summary>
        /// Returns a new instance of IHypermediaQuery.
        /// </summary>
        /// <returns></returns>
        IHypermediaQuery Clone();
    }
}