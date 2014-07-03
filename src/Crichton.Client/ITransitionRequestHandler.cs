using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    [ContractClass(typeof(ITransitionRequestHandlerContracts))]
    public interface ITransitionRequestHandler
    {
        void AddRequestFilter(ITransitionRequestFilter filter);
        Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null);
    }
}
