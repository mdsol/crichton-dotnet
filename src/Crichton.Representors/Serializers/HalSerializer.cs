using System;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HalSerializer : ISerializer
    {
        public string Serialize(CrichtonRepresentor representor)
        {
            var jObject = new JObject();

            if (!String.IsNullOrWhiteSpace(representor.SelfLink)) AddLink(jObject, "self", representor.SelfLink);

            foreach (var transition in representor.Transitions)
            {
                AddLink(jObject, transition.Rel, transition.Uri);
            }

            // add a root property for each property on data
            var dataJobject = JObject.FromObject(representor.Attributes);
            foreach (var property in dataJobject.Properties())
            {
                jObject.Add(property.Name, property.Value);
            }

            return jObject.ToString();
        }

        private static void AddLink(JObject document, string rel, string href)
        {
            if (document["_links"] == null) document.Add("_links", new JObject());

            var existingRel = document["_links"][rel];
            if (existingRel == null)
            {
                var jobject = (JObject) document["_links"];
                var linkObject = new JObject {{"href", href}};
                jobject.Add(rel, linkObject);
            }
            else
            {
                // we already have a ref. Need to convert this to an array if not already.
                var array = existingRel as JArray ?? new JArray {existingRel};
                var linkObject = new JObject { { "href", href } };
                array.Add(linkObject);

                // override the existing _links > rel
                document["_links"][rel] = array;

            }
        }

        public void DeserializeToBuilder(string message, IRepresentorBuilder builder)
        {
            var document = JObject.Parse(message);

            SetSelfLinkIfPresent(document, builder);

            CreateTransitions(document, builder);

            // set builder attributes to be that of root properties in message
            builder.SetAttributes(JObject.Parse(message));
        }

        private static void SetSelfLinkIfPresent(JObject document, IRepresentorBuilder builder)
        {
            if (document["_links"] == null) return;
            if (document["_links"]["self"] == null) return;
            if (document["_links"]["self"]["href"] == null) return;

            builder.SetSelfLink(document["_links"]["self"]["href"].Value<string>());
        }

        private static void CreateTransitions(JObject document, IRepresentorBuilder builder)
        {
            if (document["_links"] == null) return;

            foreach (var child in ((JObject) document["_links"]).Properties())
            {
                var rel = child.Name;

                var array = document["_links"][rel] as JArray;
                if (array == null)
                {
                    if (document["_links"][rel]["href"] != null)
                    {
                        // single link for this rel only
                        builder.AddTransition(rel, document["_links"][rel]["href"].Value<string>());
                    }
                }
                else
                {
                    // create a transition for each array element
                    foreach (var link in array)
                    {
                        if (link["href"] != null)
                        {
                            builder.AddTransition(rel, link["href"].Value<string>());
                        }
                    }
                }
            }
        }
    }
}
