using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    /// <summary>
    /// ITransitionRequestHandler interface
    /// </summary>
    public interface ITransitionRequestHandler
    {
        /// <summary>
        /// Adds a RequestFilter
        /// </summary>
        /// <param name="filter">the filter</param>
        void AddRequestFilter(ITransitionRequestFilter filter);

        /// <summary>
        /// Requests the transition
        /// </summary>
        /// <param name="transition">the transition</param>
        /// <param name="toSerializeToJson">the object to be posted</param>
        /// <returns>task including a CrichtonRepresentor</returns>
        Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null);
    }
}
