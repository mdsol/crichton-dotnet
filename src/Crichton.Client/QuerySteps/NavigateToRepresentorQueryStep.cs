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
        public CrichtonRepresentor Representor { get; private set; }

        public NavigateToRepresentorQueryStep(CrichtonRepresentor representor)
        {
            this.Representor = representor;
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            return Representor;
        }
    }
}
