using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class PostToTransitionQueryStep : NavigateToTransitionQueryStep
    {
        public object Data { get; private set; }

        public PostToTransitionQueryStep(string rel, object data) : base(rel)
        {
            this.Data = data;
        }

        public PostToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectorFunc, object data)
            : base(transitionSelectorFunc)
        {
            this.Data = data;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition, Data);
        }
    }
}
