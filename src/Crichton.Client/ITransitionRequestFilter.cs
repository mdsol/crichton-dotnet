using System.Net.Http;

namespace Crichton.Client
{
    /// <summary>
    /// ITransitionRequestFilter interface
    /// </summary>
    public interface ITransitionRequestFilter
    {
        /// <summary>
        /// Executes the message
        /// </summary>
        /// <param name="httpRequestMessage">the httpRequestMessage</param>
        void Execute(HttpRequestMessage httpRequestMessage);
    }
}
