using System;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    /// <summary>
    /// PostToTransitionQueryStep class
    /// </summary>
    public class PostToTransitionQueryStep : NavigateToTransitionQueryStep
    {
        /// <summary>
        /// Gets the Data
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the PostToTransitionQueryStep class.
        /// </summary>
        /// <param name="rel">the rel</param>
        /// <param name="data">the data</param>
        public PostToTransitionQueryStep(string rel, object data) : base(rel)
        {
            this.Data = data;
        }

        /// <summary>
        /// Initializes a new instance of the PostToTransitionQueryStep class.
        /// </summary>
        /// <param name="transitionSelectorFunc">the transitionSelectorFunc</param>
        /// <param name="data">the data</param>
        public PostToTransitionQueryStep(Func<CrichtonTransition, bool> transitionSelectorFunc, object data)
            : base(transitionSelectorFunc)
        {
            this.Data = data;
        }

        /// <summary>
        /// Executes the representor
        /// </summary>
        /// <param name="currentRepresentor">the currentRepresentor</param>
        /// <param name="transitionRequestHandler">the transitionRequestHandler</param>
        /// <returns>crichton representor</returns>
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            var transition = LocateTransition(currentRepresentor);

            return transitionRequestHandler.RequestTransitionAsync(transition, Data);
        }
    }
}
