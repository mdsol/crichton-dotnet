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
    /// <summary>
    /// HttpClientTransitionRequestHandler class
    /// </summary>
    public class HttpClientTransitionRequestHandler : ITransitionRequestHandler
    {
        private static readonly string[] ValidHttpMethods = new[] { "get", "post", "put", "options", "head", "delete", "trace" };
        private readonly IList<ITransitionRequestFilter> filters = new List<ITransitionRequestFilter>();

        /// <summary>
        /// Gets the HttpClient
        /// </summary>
        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// Gets the Serializer
        /// </summary>
        public ISerializer Serializer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the HttpClientTransitionRequestHandler class.
        /// </summary>
        /// <param name="client">the HttpClient</param>
        /// <param name="serializer">the serializer</param>
        public HttpClientTransitionRequestHandler(HttpClient client, ISerializer serializer)
        {
            if (client == null) { throw new ArgumentNullException("client"); }
            if (client.BaseAddress == null) { throw new ArgumentException("BaseAddress must be set on HttpClient."); }
            if (serializer == null) { throw new ArgumentNullException("serializer"); }

            HttpClient = client;
            Serializer = serializer;
        }

        /// <summary>
        /// Adds a RequestFilter
        /// </summary>
        /// <param name="filter">the filter</param>
        public void AddRequestFilter(ITransitionRequestFilter filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");

            filters.Add(filter);
        }

        /// <summary>
        /// Requests the transition
        /// </summary>
        /// <param name="transition">the transition</param>
        /// <param name="toSerializeToJson">the object to be posted</param>
        /// <returns>task including a CrichtonRepresentor</returns>
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