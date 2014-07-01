using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    public class NavigateToRelativeUrlQueryStep : IQueryStep
    {
        public string Url { get; private set; }

        public NavigateToRelativeUrlQueryStep(string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }

            this.Url = url;
        }

        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var transition = new CrichtonTransition() {Uri = Url};
            return transitionRequestHandler.RequestTransitionAsync(transition);
        }
    }
}
