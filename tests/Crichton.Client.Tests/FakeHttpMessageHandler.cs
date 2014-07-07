using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Crichton.Client.Tests
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public string Response { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public Func<HttpRequestMessage, bool> Condition { get; set; }
        public string ContentType { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Condition != null && !Condition(request))
            {
                throw new Exception("Condition did not match in returning a response message.");
            }

            var memStream = new MemoryStream();

            var sw = new StreamWriter(memStream);
            sw.Write(Response);
            sw.Flush();
            memStream.Position = 0;

            var httpContent = new StreamContent(memStream);

            var response = new HttpResponseMessage()
            {
                StatusCode = ResponseStatusCode,
                Content = httpContent
            };

            if (ContentType != null)
            {
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            }

            return response;
        }
    }
}
