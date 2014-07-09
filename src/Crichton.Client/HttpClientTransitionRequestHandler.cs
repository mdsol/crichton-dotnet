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

        private readonly IList<ITransitionRequestFilter> filters = new List<ITransitionRequestFilter>(); 

        public HttpClientTransitionRequestHandler(HttpClient client, ISerializer serializer)
        {
            if (client == null) { throw new ArgumentNullException("client"); }
            if (client.BaseAddress == null) { throw new ArgumentException("BaseAddress must be set on HttpClient."); }
            if (serializer == null) { throw new ArgumentNullException("serializer"); }

            HttpClient = client;
            Serializer = serializer;
        }

        private static readonly string[] ValidHttpMethods = new[] { "get", "post", "put", "options", "head", "delete", "trace" };

        public void AddRequestFilter(ITransitionRequestFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            filters.Add(filter);
        }

        public async Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null)
        {
            if (transition == null) { throw new ArgumentNullException("transition"); }

            var requestMessage = new HttpRequestMessage
            {
                RequestUri = transition.Uri != null ? new Uri(transition.Uri, UriKind.RelativeOrAbsolute) : null,
            };

            if (toSerializeToJson != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(toSerializeToJson), Encoding.UTF8, "application/json");
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

            // run all request filters
            foreach (var filter in filters)
            {
                filter.Execute(requestMessage);
            }

            var result = await HttpClient.SendAsync(requestMessage);

            // will throw HttpRequestException if the request fails
            result.EnsureSuccessStatusCode();

            if (result.Content.Headers.ContentType.MediaType != Serializer.ContentType)
            {
                throw new InvalidOperationException(String.Format("Response from {0} was requested with Accept header {1} but the response was {2}.", 
                    requestMessage.RequestUri, 
                    Serializer.ContentType, 
                    result.Content.Headers.ContentType.MediaType));
            }

            var resultContentString = await result.Content.ReadAsStringAsync();

            var builder = Serializer.DeserializeToNewBuilder(resultContentString, () => new RepresentorBuilder());

            return builder.ToRepresentor();
        }
    }
}