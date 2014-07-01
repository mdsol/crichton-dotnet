using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class PostToTransitionQueryStep : NavigateToTransitionQueryStep
    {
        public object Data { get; private set; }

        public PostToTransitionQueryStep(string rel, object data) : base(rel)
        {
            Contract.Requires<ArgumentNullException>(rel != null, "rel must not be null");
            Contract.Requires<ArgumentNullException>(data != null, "data must not be null");

            this.Data = data;
        }

        public PostToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectorFunc, object data)
            : base(transitionSelectorFunc)
        {
            Contract.Requires<ArgumentNullException>(transitionSelectorFunc != null, "transitionSelectorFunc must not be null");
            Contract.Requires<ArgumentNullException>(data != null, "data must not be null");

            this.Data = data;
        }

        public override Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition, Data);
        }
    }
}
