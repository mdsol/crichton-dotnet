using System.Net.Http;

namespace Crichton.Client
{
    public interface ITransitionRequestFilter
    {
        void Execute(HttpRequestMessage httpRequestMessage);
    }
}
