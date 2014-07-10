using System;
using System.Linq;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    /// <summary>
    /// NavigateToTransitionQueryStep class
    /// </summary>
    public class NavigateToTransitionQueryStep : IQueryStep
    {
        private readonly Func<CrichtonTransition, bool> selectionFunc;

        /// <summary>
        /// Initializes a new instance of the NavigateToTransitionQueryStep class.
        /// </summary>
        /// <param name="rel">the rel</param>
        public NavigateToTransitionQueryStep(string rel)
        {
            if (rel == null) { throw new ArgumentNullException("rel"); }

            selectionFunc = transition => transition.Rel == rel;
        }

        /// <summary>
        /// Initializes a new instance of the NavigateToTransitionQueryStep class.
        /// </summary>
        /// <param name="transitionSelectionFunc">the transitionSelectionFunc</param>
        public NavigateToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectionFunc)
        {
            if (transitionSelectionFunc == null) { throw new ArgumentNullException("transitionSelectionFunc"); }

            selectionFunc = transitionSelectionFunc;
        }

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

            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition);
        }

        public CrichtonTransition LocateTransition(CrichtonRepresentor currentRepresentor)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }

            var transition = currentRepresentor.Transitions.Single(selectionFunc);
            return transition;
        }
    }
}
