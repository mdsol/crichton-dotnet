using System;
using System.Net.Http;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.Client
{
    /// <summary>
    /// Crichton Client class
    /// </summary>
    public class CrichtonClient
    {
        /// <summary>
        /// Gets the TransitionRequestHandler.
        /// </summary>
        public ITransitionRequestHandler TransitionRequestHandler { get; private set; }

        /// <summary>
        /// Initializes a new instance of the CrichtonClient class.
        /// </summary>
        /// <param name="transitionRequestHandler">the transitionRequestHandler</param>
        public CrichtonClient(ITransitionRequestHandler transitionRequestHandler)
        {
            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            TransitionRequestHandler = transitionRequestHandler;
        }

        /// <summary>
        /// Initializes a new instance of the CrichtonClient class.
        /// </summary>
        /// <param name="baseAddress">the baseAddress</param>
        /// <param name="serializer">the serializer</param>
        public CrichtonClient(Uri baseAddress, ISerializer serializer) : this(new HttpClient{BaseAddress = baseAddress}, serializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CrichtonClient class.
        /// </summary>
        /// <param name="client">the client</param>
        /// <param name="serializer">the serializer</param>
        public CrichtonClient(HttpClient client, ISerializer serializer)
        {
            if (client == null) { throw new ArgumentNullException("client"); }
            if (client.BaseAddress == null) { throw new ArgumentException("HttpClient.BaseAddress must not be null."); }
            if (serializer == null) { throw new ArgumentNullException("serializer"); }

            TransitionRequestHandler = new HttpClientTransitionRequestHandler(client, serializer);
        }

        /// <summary>
        /// Creates a query
        /// </summary>
        /// <param name="representor">the representor</param>
        /// <returns>created query</returns>
        public IHypermediaQuery CreateQuery(CrichtonRepresentor representor = null)
        {
            var query = new HypermediaQuery();
            if (representor == null)
            {
                return query;
            }

            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }

        /// <summary>
        /// Executes the query
        /// </summary>
        /// <param name="query">the query</param>
        /// <returns>result of the query</returns>
        public Task<CrichtonRepresentor> ExecuteQueryAsync(IHypermediaQuery query)
        {
            if (query == null) { throw new ArgumentNullException("query"); }

            return query.ExecuteAsync(TransitionRequestHandler);
        }
    }
}
