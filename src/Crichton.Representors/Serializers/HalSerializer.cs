using System;
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

            if (!String.IsNullOrWhiteSpace(representor.SelfLink)) AddLink(jObject, "self", representor.SelfLink, null);

            foreach (var transition in representor.Transitions.Where(t => !ReservedLinkRels.Contains(t.Rel)))
            {
                AddLink(jObject, transition.Rel, transition.Uri, transition.Title);
            }

            // add a root property for each property on data
            foreach (var property in representor.Attributes.Properties().Where(p => !ReservedAttributes.Contains(p.Name)))
            {
                jObject.Add(property.Name, property.Value);
            }

            // create embedded resources
            if (representor.EmbeddedResources.Any())
            {
                var embeddedJObject = new JObject();

                foreach (var embeddedResourceKey in representor.EmbeddedResources.Keys)
                {
                    var list = representor.EmbeddedResources[embeddedResourceKey];
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

                jObject.Add("_embedded", embeddedJObject);
            }

            return jObject;
        }

        private static void AddLink(JObject document, string rel, string href, string title)
        {
            if (document["_links"] == null) document.Add("_links", new JObject());

            var existingRel = document["_links"][rel];
            if (existingRel == null)
            {
                var jobject = (JObject) document["_links"];
                var linkObject = new JObject {{"href", href}};
                if (!String.IsNullOrWhiteSpace(title)) linkObject["title"] = title;
                jobject.Add(rel, linkObject);
            }
            else
            {
                // we already have a ref. Need to convert this to an array if not already.
                var array = existingRel as JArray ?? new JArray {existingRel};
                var linkObject = new JObject { { "href", href } };
                if (!String.IsNullOrWhiteSpace(title)) linkObject["title"] = title;
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
                var propertyAsArray = embedded[property.Name] as JArray;
                if (propertyAsArray != null)
                {
                    // multiple items in same embedded resource key as an array
                    foreach (var item in propertyAsArray.OfType<JObject>())
                    {
                        var builderResult = BuildRepresentorBuilderFromJObject(builderFactoryMethod, item);
                        currentBuilder.AddEmbeddedResource(property.Name, builderResult.ToRepresentor());
                    }
                }
                else
                {
                    // single item under resource key
                    var builderResult = BuildRepresentorBuilderFromJObject(builderFactoryMethod, (JObject)embedded[property.Name]);
                    currentBuilder.AddEmbeddedResource(property.Name, builderResult.ToRepresentor());
                }
            }
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

                        // single link for this rel only
                        builder.AddTransition(rel, document["_links"][rel]["href"].Value<string>(), (title == null) ? null : title.Value<string>());
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

                            builder.AddTransition(rel, link["href"].Value<string>(), (title == null) ? null : title.Value<string>());
                        }
                    }
                }
            }
        }
    }
}
