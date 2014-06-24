using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToTransitionQueryStep : IQueryStep
    {
        private readonly Func<CrichtonTransition, bool> selectionFunc;

        public NavigateToTransitionQueryStep(string rel)
        {
            selectionFunc = transition => transition.Rel == rel;
        }

        public NavigateToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectionFunc)
        {
            selectionFunc = transitionSelectionFunc;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestor transitionRequestor)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestor.RequestTransitionAsync(transition);
        }

        internal CrichtonTransition LocateTransition(CrichtonRepresentor currentRepresentor)
        {
            var transition = currentRepresentor.Transitions.Single(selectionFunc);
            return transition;
        }
    }
}
