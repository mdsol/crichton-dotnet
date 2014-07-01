using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToSelfLinkQueryStep : IQueryStep
    {
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var selfTransition = new CrichtonTransition() {Uri = currentRepresentor.SelfLink};

            return transitionRequestHandler.RequestTransitionAsync(selfTransition);
        }
    }
}
