using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using Crichton.Representors;
using Crichton.Representors.Serializers;

namespace Crichton.WebApi
{
    public class CrichtonRepresentorBuilderMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        private static readonly IReadOnlyDictionary<string, ISerializer> Serializers = new ReadOnlyDictionary
            <string, ISerializer>(
            new Dictionary<string, ISerializer>()
            {
                {"application/hal+json", new HalSerializer()},
                {"application/hale+json", new HaleSerializer()}
            });


        public CrichtonRepresentorBuilderMediaTypeFormatter()
        {
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
            return type == typeof (IRepresentorBuilder);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var serializer = Serializers[content.Headers.ContentType.MediaType];

            using (var writer = new StreamWriter(writeStream))
            {
                writer.Write(serializer.Serialize(((IRepresentorBuilder)value).ToRepresentor()));
            }

        }
    }
}
