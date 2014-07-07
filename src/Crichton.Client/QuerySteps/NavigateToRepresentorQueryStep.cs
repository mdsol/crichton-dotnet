using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToRepresentorQueryStep : IQueryStep
    {
        public CrichtonRepresentor Representor { get; private set; }

        public NavigateToRepresentorQueryStep(CrichtonRepresentor representor)
        {
            if (representor == null) { throw new ArgumentNullException("representor"); }

            this.Representor = representor;
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            return Representor;
        }
    }
}
