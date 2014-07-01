using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client.QuerySteps
{
    [ContractClassFor(typeof(IQueryStep))]
    internal abstract class IQueryStepContracts : IQueryStep
    {
        public Task<CrichtonRepresentor> ExecuteAsync(CrichtonRepresentor currentRepresentor, ITransitionRequestHandler transitionRequestHandler)
        {
            Contract.Requires(currentRepresentor != null, "currentRepresentor must not be null.");
            Contract.Requires(transitionRequestHandler != null, "transitionRequestHandler must not be null.");
            Contract.Ensures(Contract.Result<Task<CrichtonRepresentor>>() != null, "Return value must not be null.");

            return default(Task<CrichtonRepresentor>);
        }
    }
}
