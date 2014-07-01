using System;
using System.Diagnostics.Contracts;
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
            Contract.Requires(transitionRequestHandler != null, "transitionRequestHandler must not be null");

            TransitionRequestHandler = transitionRequestHandler;
        }

        public CrichtonClient(Uri baseAddress, ISerializer serializer) : this(new HttpClient{BaseAddress = baseAddress}, serializer)
        {
            Contract.Requires(baseAddress != null, "baseAddress must not be null");
            Contract.Requires(serializer != null, "serializer must not be null");
        }

        public CrichtonClient(HttpClient client, ISerializer serializer)
        {
            Contract.Requires(client != null, "client must not be null");
            Contract.Requires(client.BaseAddress != null, "client.BaseAddress must not be null");
            Contract.Requires(serializer != null, "serializer must not be null");

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
            Contract.Requires(query != null, "query must not be null");

            return query.ExecuteAsync(TransitionRequestHandler);
        }
    }
}
