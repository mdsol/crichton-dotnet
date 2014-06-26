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
        public string Url { get; private set; }

        public NavigateToRelativeUrlQueryStep(string url)
        {
            this.Url = url;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = new CrichtonTransition() {Uri = Url};
            return transitionRequestHandler.RequestTransitionAsync(transition);
        }
    }
}
