using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;
using Crichton.Representors.Serializers;
using Newtonsoft.Json;

namespace Crichton.Client
{
    public class HttpClientTransitionRequestHandler : ITransitionRequestHandler
    {
        public HttpClient HttpClient { get; private set; }
        public ISerializer Serializer { get; private set; }

        public HttpClientTransitionRequestHandler(HttpClient client, ISerializer serializer)
        {
            if (client.BaseAddress == null)
            {
                throw new InvalidOperationException("BaseAddress must be set on HttpClient.");
            }

            HttpClient = client;
            Serializer = serializer;
        }

        private static readonly string[] ValidHttpMethods = new[] { "get", "post", "put", "options", "head", "delete", "trace" };

        public async Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null)
        {
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(transition.Uri, UriKind.RelativeOrAbsolute)
            };

            if (toSerializeToJson != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(toSerializeToJson));
            }

            // select HttpMethod based on if there is data to serialize or not
            requestMessage.Method = toSerializeToJson == null ? HttpMethod.Get : HttpMethod.Post;

            if (transition.Methods != null && transition.Methods.Any() && ValidHttpMethods.Contains(transition.Methods.First().ToLowerInvariant()))
            {
                // an HttpMethod has been specified in the transition. Override it in the request.
                requestMessage.Method = new HttpMethod(transition.Methods.First().ToUpperInvariant());
            }

            // add Accept header
            requestMessage.Headers.Accept.ParseAdd(Serializer.ContentType);

            var result = await HttpClient.SendAsync(requestMessage);

            var resultContentString = await result.Content.ReadAsStringAsync();

            var builder = Serializer.DeserializeToNewBuilder(resultContentString, () => new RepresentorBuilder());

            return builder.ToRepresentor();
        }
    }
}
