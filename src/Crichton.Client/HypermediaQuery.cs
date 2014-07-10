using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    /// <summary>
    /// HypermediaQuery class
    /// </summary>
    public class HypermediaQuery : IHypermediaQuery
    {
        public IList<IQueryStep> Steps { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HypermediaQuery class.
        /// </summary>
        public HypermediaQuery() : this(new List<IQueryStep>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the HypermediaQuery class.
        /// </summary>
        /// <param name="steps">query steps</param>
        private HypermediaQuery(IEnumerable<IQueryStep> steps)
        {
            Steps = steps.ToList();
        }

        /// <summary>
        /// Add QueryStep
        /// </summary>
        /// <param name="step">query step</param>
        public void AddStep(IQueryStep step)
        {
            if (step == null) { throw new ArgumentNullException("step"); }

            Steps.Add(step);
        }

        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="requestHandler">the requestHandler</param>
        /// <returns>task including a CrichtonRepresentor</returns>
        public async Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestHandler requestHandler)
        {
            if (requestHandler == null) { throw new ArgumentNullException("requestHandler"); }

            CrichtonRepresentor representor = null;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var step in Steps)
            {
                representor = await step.ExecuteAsync(representor, requestHandler);
            }

            return representor;
        }

        /// <summary>
        /// Returns a new instance of HypermediaQuery.
        /// </summary>
        /// <returns></returns>
        public IHypermediaQuery Clone()
        {
            return new HypermediaQuery(this.Steps);
        }
    }
}
