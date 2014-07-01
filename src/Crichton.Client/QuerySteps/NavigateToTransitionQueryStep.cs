using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToTransitionQueryStep : IQueryStep
    {
        private readonly Func<CrichtonTransition, bool> selectionFunc;

        public NavigateToTransitionQueryStep(string rel)
        {
            Contract.Requires(rel != null, "rel must not be null");

            selectionFunc = transition => transition.Rel == rel;
        }

        public NavigateToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectionFunc)
        {
            Contract.Requires(transitionSelectionFunc != null, "transitionSelectionFunc must not be null");

            selectionFunc = transitionSelectionFunc;
        }

        public virtual Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition);
        }

        public CrichtonTransition LocateTransition(CrichtonRepresentor currentRepresentor)
        {
            Contract.Requires(currentRepresentor != null, "currentRepresentor must not be null");

            var transition = currentRepresentor.Transitions.Single(selectionFunc);
            return transition;
        }
    }
}
