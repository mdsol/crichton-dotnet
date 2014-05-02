using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HalSerializer : ISerializer
    {
        private static readonly string[] ReservedAttributes = {"_links", "_embedded"};
        private static readonly string[] ReservedLinkRels = {"self"};

        public string Serialize(CrichtonRepresentor representor)
        {
            var jObject = CreateJObjectForRepresentor(representor);

            return jObject.ToString();
        }

        private static JObject CreateJObjectForRepresentor(CrichtonRepresentor representor)
        {
            var jObject = new JObject();

            if (!String.IsNullOrWhiteSpace(representor.SelfLink)) AddLink(jObject, "self", representor.SelfLink, null, null);

            foreach (var transition in representor.Transitions.Where(t => !ReservedLinkRels.Contains(t.Rel)))
            {
                AddLink(jObject, transition.Rel, transition.Uri, transition.Title, transition.Type);
            }

            // add a root property for each property on data
            foreach (var property in representor.Attributes.Properties().Where(p => !ReservedAttributes.Contains(p.Name)))
            {
                jObject.Add(property.Name, property.Value);
            }

            // embedded resources and Collections both require _embedded
            if (representor.EmbeddedResources.Any() || representor.Collection.Any())
            {
                var embeddedJObject = new JObject();
                jObject.Add("_embedded", embeddedJObject);

                // create embedded resources
                foreach (var embeddedResourceKey in representor.EmbeddedResources.Keys)
                {
                    var list = representor.EmbeddedResources[embeddedResourceKey];
                    AddRepresentorsToEmbedded(list, embeddedJObject, embeddedResourceKey);
                }

                // add Collection objects to _embedded.items
                AddRepresentorsToEmbedded(representor.Collection.ToList(), embeddedJObject, "items");
            }

            return jObject;
        }

        private static void AddRepresentorsToEmbedded(IList<CrichtonRepresentor> list, JObject embeddedJObject, string embeddedResourceKey)
        {
            if (list.Count == 1)
            {
                // single embedded resources are resources
                embeddedJObject.Add(embeddedResourceKey, CreateJObjectForRepresentor(list.Single()));
            }
            else if (list.Count > 1)
            {
                // multiple embedded resources are an array of resources
                var array = new JArray();
                foreach (var resource in list)
                {
                    array.Add(CreateJObjectForRepresentor(resource));
                }
                embeddedJObject.Add(embeddedResourceKey, array);
            }
        }

        private static void AddLink(JObject document, string rel, string href, string title, string type)
        {
            if (document["_links"] == null) document.Add("_links", new JObject());

            var existingRel = document["_links"][rel];
            if (existingRel == null)
            {
                var jobject = (JObject) document["_links"];
                var linkObject = new JObject {{"href", href}};
                if (!String.IsNullOrWhiteSpace(title)) linkObject["title"] = title;
                if (!String.IsNullOrWhiteSpace(type)) linkObject["type"] = type;
                jobject.Add(rel, linkObject);
            }
            else
            {
                // we already have a ref. Need to convert this to an array if not already.
                var array = existingRel as JArray ?? new JArray {existingRel};
                var linkObject = new JObject { { "href", href } };
                if (!String.IsNullOrWhiteSpace(title)) linkObject["title"] = title;
                if (!String.IsNullOrWhiteSpace(type)) linkObject["type"] = type;
                array.Add(linkObject);

                // override the existing _links > rel
                document["_links"][rel] = array;

            }
        }

        public IRepresentorBuilder DeserializeToNewBuilder(string message, Func<IRepresentorBuilder> builderFactoryMethod)
        {
            var document = JObject.Parse(message);
            
            var builder = BuildRepresentorBuilderFromJObject(builderFactoryMethod, document);

            return builder;
        }

        private IRepresentorBuilder BuildRepresentorBuilderFromJObject(Func<IRepresentorBuilder> builderFactoryMethod, JObject document)
        {
            var builder = builderFactoryMethod();

            SetSelfLinkIfPresent(document, builder);

            CreateTransitions(document, builder);

            CreateEmbeddedResources(document, builder, builderFactoryMethod);

            // set builder attributes to be that of root properties in message
            builder.SetAttributes(document);

            return builder;
        }

        private void CreateEmbeddedResources(JObject document, IRepresentorBuilder currentBuilder,
            Func<IRepresentorBuilder> builderFactoryMethod)
        {
            if (document["_embedded"] == null) return;

            var embedded = (JObject)document["_embedded"];

            foreach (var property in embedded.Properties())
            {
                var propertyJObject = embedded[property.Name];

                if (property.Name == "items") // collections use the "items" embedded resource key by convention
                {
                    currentBuilder.SetCollection(GetRepresentorsFromEmbeddedJObject(builderFactoryMethod, propertyJObject));
                }
                else
                {
                    foreach (var representor in GetRepresentorsFromEmbeddedJObject(builderFactoryMethod, propertyJObject))
                    {
                        currentBuilder.AddEmbeddedResource(property.Name, representor);
                    }
                }
            }
        }

        private IEnumerable<CrichtonRepresentor> GetRepresentorsFromEmbeddedJObject(Func<IRepresentorBuilder> builderFactoryMethod, JToken propertyJToken)
        {
            var representorsInCollection = new List<CrichtonRepresentor>();
            var propertyAsArray = propertyJToken as JArray;
            if (propertyAsArray != null)
            {
                // multiple items in same embedded resource an array
                representorsInCollection.AddRange(propertyAsArray.OfType<JObject>()
                    .Select(item => BuildRepresentorBuilderFromJObject(builderFactoryMethod, item))
                    .Select(builderResult => builderResult.ToRepresentor()));
            }
            else
            {
                // single item as embedded resource
                var builderResult = BuildRepresentorBuilderFromJObject(builderFactoryMethod,
                    (JObject)propertyJToken);
                representorsInCollection.Add(builderResult.ToRepresentor());
            }
            return representorsInCollection;
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
                        var title = document["_links"][rel]["title"];
                        var type = document["_links"][rel]["type"];

                        // single link for this rel only
                        builder.AddTransition(rel, document["_links"][rel]["href"].Value<string>(),
                            (title == null) ? null : title.Value<string>(),
                            (type == null) ? null : type.Value<string>());
                    }
                }
                else
                {
                    // create a transition for each array element
                    foreach (var link in array)
                    {
                        if (link["href"] != null)
                        {
                            var title = link["title"];
                            var type = link["type"];

                            builder.AddTransition(rel, link["href"].Value<string>(),
                                (title == null) ? null : title.Value<string>(),
                                (type == null) ? null : type.Value<string>());
                        }
                    }
                }
            }
        }
    }
}
