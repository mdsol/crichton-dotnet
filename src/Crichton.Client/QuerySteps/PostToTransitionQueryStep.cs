using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class PostToTransitionQueryStep : NavigateToTransitionQueryStep
    {
        private readonly object data;

        public PostToTransitionQueryStep(string rel, object data) : base(rel)
        {
            this.data = data;
        }

        public PostToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectorFunc, object data)
            : base(transitionSelectorFunc)
        {
            this.data = data;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestor transitionRequestor)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestor.PostTransitionDataAsJsonAsync(transition, data);
        }
    }
}
