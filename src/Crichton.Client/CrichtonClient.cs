using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using Crichton.Representors.Serializers;
using Newtonsoft.Json;

namespace Crichton.Client
{
    public class CrichtonClient
    {
        public ITransitionRequestHandler TransitionRequestHandler { get; private set; }

        public CrichtonClient(ITransitionRequestHandler transitionRequestHandler = null)
        {
            TransitionRequestHandler = transitionRequestHandler;
        }

        public CrichtonClient(Uri baseAddress, ISerializer serializer)
        {
            var httpClient = new HttpClient {BaseAddress = baseAddress};
            TransitionRequestHandler = new HttpClientTransitionRequestHandler(httpClient, serializer);
        }

        public IHypermediaQuery CreateQuery()
        {
            return new HypermediaQuery();
        }

        public IHypermediaQuery CreateQuery(CrichtonRepresentor representor)
        {
            var query = new HypermediaQuery();
            query.AddStep(new NavigateToRepresentorQueryStep(representor));
            return query;
        }

        public Task<CrichtonRepresentor> ExecuteQueryAsync(IHypermediaQuery query)
        {
            return query.ExecuteAsync(TransitionRequestHandler);
        }
    }
}
