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

            HttpClient.BaseAddress = baseUri;
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
            return query.ExecuteAsync(this);
        }

        private async Task<CrichtonRepresentor> SendTransitionRequestAsync(CrichtonTransition transition,
            HttpMethod httpMethod, object data)
        {
            var requestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(transition.Uri, UriKind.RelativeOrAbsolute)
            };

            if (data != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data));
            }

            var result = await HttpClient.SendAsync(requestMessage);

            var resultContentString = await result.Content.ReadAsStringAsync();

            var builder = Serializer.DeserializeToNewBuilder(resultContentString, () => new RepresentorBuilder());

            return builder.ToRepresentor();
        }

        public Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition)
        {
            return SendTransitionRequestAsync(transition, HttpMethod.Get, null);
        }

        public Task<CrichtonRepresentor> PostTransitionDataAsJsonAsync(CrichtonTransition transition, object toSerializeToJson)
        {
            return SendTransitionRequestAsync(transition, HttpMethod.Post, toSerializeToJson);
        }
    }
}
