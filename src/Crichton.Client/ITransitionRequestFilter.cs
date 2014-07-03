using System.Diagnostics.Contracts;
using System.Net.Http;

namespace Crichton.Client
{
    [ContractClass(typeof(ITransitionRequestFilterContracts))]
    public interface ITransitionRequestFilter
    {
        void Execute(HttpRequestMessage httpRequestMessage);
    }
}
