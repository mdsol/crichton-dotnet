using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    /// <summary>
    /// NavigateToRelativeUrlQueryStep class
    /// </summary>
    public class NavigateToRelativeUrlQueryStep : IQueryStep
    {
        public string Url { get; private set; }

        /// <summary>
        /// Initializes a new instance of the NavigateToRelativeUrlQueryStep class.
        /// </summary>
        /// <param name="url">the url</param>
        public NavigateToRelativeUrlQueryStep(string url)
        {
            if (url == null) { throw new ArgumentNullException("url"); }

            this.Url = url;
        }

        /// <summary>
        /// Executes the representor
        /// </summary>
        /// <param name="currentRepresentor">the currentRepresentor</param>
        /// <param name="transitionRequestHandler">the transitionRequestHandler</param>
        /// <returns>crichton representor</returns>
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            if (currentRepresentor == null) { throw new ArgumentNullException("currentRepresentor"); }
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            var transition = new CrichtonTransition() {Uri = Url};
            return transitionRequestHandler.RequestTransitionAsync(transition);
        }
    }
}
