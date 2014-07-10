using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    /// <summary>
    /// NavigateToRepresentorQueryStep class
    /// </summary>
    public class NavigateToRepresentorQueryStep : IQueryStep
    {
        /// <summary>
        /// Gets the Representor
        /// </summary>
        public CrichtonRepresentor Representor { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NavigateToRelativeUrlQueryStep class.
        /// </summary>
        /// <param name="representor">the representor</param>
        public NavigateToRepresentorQueryStep(CrichtonRepresentor representor)
        {
            if (representor == null) { throw new ArgumentNullException("representor"); }

            this.Representor = representor;
        }

        /// <summary>
        /// Executes the representor
        /// </summary>
        /// <param name="currentRepresentor">the currentRepresentor</param>
        /// <param name="transitionRequestHandler">the transitionRequestHandler</param>
        /// <returns>crichton representor</returns>
        public async Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            return Representor;
        }
    }
}
