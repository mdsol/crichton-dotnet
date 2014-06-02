using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.WebApi
{
    public class CrichtonMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly IReadOnlyDictionary<string, ISerializer> Serializers = new ReadOnlyDictionary
            <string, ISerializer>(
            new Dictionary<string, ISerializer>()
            {
                {"application/hal+json", new HalSerializer()},
                {"application/hale+json", new HaleSerializer()}
            });

        private readonly List<IBuilderDescriptor> descriptors = new List<IBuilderDescriptor>();

        private readonly HttpRequestMessage requestMessage;

        public CrichtonMediaTypeFormatter(params IBuilderDescriptor[] descriptors) : this(null, descriptors)
        {
        }

        public CrichtonMediaTypeFormatter(HttpRequestMessage requestMessage, params IBuilderDescriptor[] descriptors)
        {
            this.requestMessage = requestMessage;

            this.descriptors.AddRange(descriptors);

            foreach (var serializer in Serializers)
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(serializer.Key));
            }
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof (IRepresentorBuilder) || descriptors.Any(d => d.SupportsType(type));
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new CrichtonMediaTypeFormatter(request, descriptors.ToArray());
        }

        public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var serializer = Serializers[content.Headers.ContentType.MediaType];
            
            // Support serializing of returned IRepresentorBuilders
            if (type == typeof (IRepresentorBuilder))
            {
                await WriteToStreamFromBuilder((IRepresentorBuilder)value, writeStream, serializer);
            }
            else if (requestMessage != null)
            {
                // Support a matching Descriptor
                var matchingDescriptor = descriptors.Single(d => d.SupportsType(type));

                var requestContext = (HttpRequestContext)requestMessage.Properties[HttpPropertyKeys.RequestContextKey];
                var builder = matchingDescriptor.BuildForType(type, value, requestContext);
                await WriteToStreamFromBuilder(builder, writeStream, serializer);

            }
        }

        private static async Task WriteToStreamFromBuilder(IRepresentorBuilder builder, Stream writeStream, ISerializer serializer)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                await writer.WriteAsync(serializer.Serialize(builder.ToRepresentor()));
            }
        }
    }
}
