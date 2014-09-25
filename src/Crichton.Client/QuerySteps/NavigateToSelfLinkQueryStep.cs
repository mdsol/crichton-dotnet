using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    /// <summary>
    /// NavigateToSelfLinkQueryStep class
    /// </summary>
    public class NavigateToSelfLinkQueryStep : IQueryStep
    {
        /// <summary>
        /// Executes the representor
        /// </summary>
        /// <param name="currentRepresentor">the currentRepresentor</param>
        /// <param name="transitionRequestHandler">the transitionRequestHandler</param>
        /// <returns>crichton representor</returns>
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var selfTransition = new CrichtonTransition() {Uri = currentRepresentor.SelfLink};

            return transitionRequestHandler.RequestTransitionAsync(selfTransition);
        }
    }
}
