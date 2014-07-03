using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    [ContractClassFor(typeof(ITransitionRequestHandler))]
    internal abstract class ITransitionRequestHandlerContracts : ITransitionRequestHandler
    {
        public void AddRequestFilter(ITransitionRequestFilter filter)
        {
            Contract.Requires(filter != null, "filter must not be null.");   
        }

        public Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null)
        {
            Contract.Requires(transition != null, "transition must not be null.");
            Contract.Ensures(Contract.Result<Task<CrichtonRepresentor>>() != null, "Return value must not be null.");

            return default(Task<CrichtonRepresentor>);
        }
    }
}
