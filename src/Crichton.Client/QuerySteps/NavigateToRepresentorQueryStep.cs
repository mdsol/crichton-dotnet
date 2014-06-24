using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToRepresentorQueryStep : IQueryStep
    {
        private CrichtonRepresentor representor;

        public NavigateToRepresentorQueryStep(CrichtonRepresentor representor)
        {
            this.representor = representor;
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestor transitionRequestor)
        {
            return representor;
        }
    }
}
