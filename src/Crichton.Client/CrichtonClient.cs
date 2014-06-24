using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.Client
{
    public class CrichtonClient : ITransitionRequestor
    {
        public HttpClient HttpClient { get; private set; }
        public ISerializer Serializer { get; private set; }
        public Uri BaseUri { get; private set; }

        public CrichtonClient(HttpClient httpClient, Uri baseUri, ISerializer serializer)
        {
            HttpClient = httpClient;
            Serializer = serializer;
            BaseUri = baseUri;
        }

        public IHypermediaQuery CreateQuery()
        {
            return new HypermediaQuery();
        }

        public Task<CrichtonRepresentor> ExecuteQueryAsync(IHypermediaQuery query)
        {
            return query.ExecuteAsync(this);
        }
    }
}
