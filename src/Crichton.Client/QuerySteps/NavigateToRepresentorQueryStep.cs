using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToRepresentorQueryStep : IQueryStep
    {
        public CrichtonRepresentor Representor { get; private set; }

        public NavigateToRepresentorQueryStep(CrichtonRepresentor representor)
        {
            Contract.Requires<ArgumentNullException>(representor != null, "representor must not be null"); 

            this.Representor = representor;
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            return Representor;
        }
    }
}
