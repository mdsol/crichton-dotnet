using System;
using System.Net.Http;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.Client
{
    public class CrichtonClient
    {
        public ITransitionRequestHandler TransitionRequestHandler { get; private set; }

        public CrichtonClient(ITransitionRequestHandler transitionRequestHandler)
        {

            if (transitionRequestHandler == null) { throw new ArgumentNullException("transitionRequestHandler"); }

            TransitionRequestHandler = transitionRequestHandler;
        }

        public CrichtonClient(Uri baseAddress, ISerializer serializer) : this(new HttpClient{BaseAddress = baseAddress}, serializer)
        {
        }

        public CrichtonClient(HttpClient client, ISerializer serializer)
        {
            if (client == null) { throw new ArgumentNullException("client"); }
            if (client.BaseAddress == null) { throw new ArgumentException("HttpClient.BaseAddress must not be null."); }
            if (serializer == null) { throw new ArgumentNullException("serializer"); }

            TransitionRequestHandler = new HttpClientTransitionRequestHandler(client, serializer);
        }

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

        public Task<CrichtonRepresentor> ExecuteQueryAsync(IHypermediaQuery query)
        {
            if (query == null) { throw new ArgumentNullException("query"); }

            return query.ExecuteAsync(TransitionRequestHandler);
        }
    }
}
