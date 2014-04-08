using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crichton.Representors.Serializers.Hal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HalSerializer
    {
        public string Serialize<T>(CrichtonRepresentor<T> representor)
        {
            var document = new HalDocument();
            
            if (String.IsNullOrWhiteSpace(representor.SelfLink)) throw new InvalidOperationException("HAL Specification requires Self Link to be set.");

            // set Self link
            document.Links = new Dictionary<string, Link> {{"self", new Link(representor.SelfLink)}};

            var jObject = JObject.FromObject(document);

            // add a root property for each property on data
            var dataJobject = JObject.FromObject(representor.Data);
            foreach (var property in dataJobject.Properties())
            {
                jObject.Add(property.Name, property.Value);
            }

            return jObject.ToString();
        }

        public CrichtonRepresentor<T> Deserialize<T>(string message)
        {
            var representor = new CrichtonRepresentor<T>();

            var document = JsonConvert.DeserializeObject<HalDocument>(message);

            ValidateDeserializedHalDocument(document);

            representor.SelfLink = document.Links["self"].Href;

            // deserialize using root properties on Hal document
            representor.Data = JsonConvert.DeserializeObject<T>(message);

            return representor;
        }

        private static void ValidateDeserializedHalDocument(HalDocument document)
        {
            const string message = "HAL Specification requires _links with self and href";

            if (document.Links == null)
                throw new InvalidOperationException(message);
            if (!document.Links.ContainsKey("self"))
                throw new InvalidOperationException(message);
            if (String.IsNullOrWhiteSpace(document.Links["self"].Href))
                throw new InvalidOperationException(message);
        }
    }
}
