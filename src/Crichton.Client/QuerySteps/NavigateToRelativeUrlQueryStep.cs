using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToRelativeUrlQueryStep : IQueryStep
    {
        private string url;

        public NavigateToRelativeUrlQueryStep(string url)
        {
            this.url = url;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = new CrichtonTransition() {Uri = url};
            return transitionRequestHandler.RequestTransitionAsync(transition);
        }
    }
}
