using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Crichton.Representors.Serializers;

namespace Crichton.Client
{
    public class CrichtonClient
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
    }
}
