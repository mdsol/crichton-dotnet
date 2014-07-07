using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    public interface ITransitionRequestHandler
    {
        void AddRequestFilter(ITransitionRequestFilter filter);
        Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null);
    }
}
