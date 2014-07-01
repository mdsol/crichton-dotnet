using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToSelfLinkQueryStep : IQueryStep
    {
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var selfTransition = new CrichtonTransition() {Uri = currentRepresentor.SelfLink};

            return transitionRequestHandler.RequestTransitionAsync(selfTransition);
        }
    }
}
