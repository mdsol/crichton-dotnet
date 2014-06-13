using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HalSerializer : ISerializer
    {
        private static readonly string[] ReservedAttributes = { "_links", "_embedded" };
        private static readonly string[] ReservedLinkRels = { "self" };

        public virtual string ContentType { get { return "application/hal+json"; } }

        public string Serialize(CrichtonRepresentor representor)
        {
            var jObject = CreateJObjectForRepresentor(representor);

            return jObject.ToString();
        }

        private JObject CreateJObjectForRepresentor(CrichtonRepresentor representor)
        {
            var jObject = new JObject();

            if (!String.IsNullOrWhiteSpace(representor.SelfLink)) AddLinkFromTransition(jObject, new CrichtonTransition { Rel = "self", Uri = representor.SelfLink });

            foreach (var transition in representor.Transitions.Where(t => !ReservedLinkRels.Contains(t.Rel)))
            {
                AddLinkFromTransition(jObject, transition);
            }

            // add a root property for each property on data
            if (representor.Attributes != null)
            {
                foreach (var property in representor.Attributes.Properties().Where(p => !ReservedAttributes.Contains(p.Name)))
                {
                    jObject.Add(property.Name, property.Value);
                }
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

        private void AddRepresentorsToEmbedded(IList<CrichtonRepresentor> list, JObject embeddedJObject, string embeddedResourceKey)
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

        private void AddLinkFromTransition(JObject document, CrichtonTransition transition)
        {
            if (document["_links"] == null) document.Add("_links", new JObject());

            var existingRel = document["_links"][transition.Rel];
            if (existingRel == null)
            {
                var jobject = (JObject)document["_links"];
                jobject.Add(transition.Rel, CreateLinkObjectFromTransition(transition));
            }
            else
            {
                // we already have a ref. Need to convert this to an array if not already.
                var array = existingRel as JArray ?? new JArray { existingRel };
                array.Add(CreateLinkObjectFromTransition(transition));

                // override the existing _links > rel
                document["_links"][transition.Rel] = array;
            }
        }

        public virtual JObject CreateLinkObjectFromTransition(CrichtonTransition transition)
        {
            var linkObject = new JObject { { "href", transition.Uri } };
            if (!String.IsNullOrWhiteSpace(transition.Title)) linkObject["title"] = transition.Title;
            if (!String.IsNullOrWhiteSpace(transition.Type)) linkObject["type"] = transition.Type;
            if (transition.UriIsTemplated) linkObject["templated"] = true;
            if (!String.IsNullOrWhiteSpace(transition.DepreciationUri)) linkObject["deprecation"] = transition.DepreciationUri;
            if (!String.IsNullOrWhiteSpace(transition.Name)) linkObject["name"] = transition.Name;
            if (!String.IsNullOrWhiteSpace(transition.ProfileUri)) linkObject["profile"] = transition.ProfileUri;
            if (!String.IsNullOrWhiteSpace(transition.LanguageTag)) linkObject["hreflang"] = transition.LanguageTag;

            return linkObject;
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

        private void CreateTransitions(JObject document, IRepresentorBuilder builder)
        {
            if (document["_links"] == null) return;

            foreach (var child in ((JObject)document["_links"]).Properties())
            {
                var rel = child.Name;

                var array = document["_links"][rel] as JArray;
                if (array == null)
                {
                    // single link for this rel only
                    builder.AddTransition(GetTransitionFromLinkObject(document["_links"][rel], rel));
                }
                else
                {
                    // create a transition for each array element
                    foreach (var link in array)
                    {
                        builder.AddTransition(GetTransitionFromLinkObject(link, rel));
                    }
                }
            }
        }

        public virtual CrichtonTransition GetTransitionFromLinkObject(JToken link, string rel)
        {
            var href = link["href"];
            var title = link["title"];
            var type = link["type"];
            var templatedField = link["templated"];
            var deprecated = link["deprecation"];
            var name = link["name"];
            var profile = link["profile"];
            var hreflang = link["hreflang"];
            var templated = templatedField != null && (bool)templatedField;

            var transition = new CrichtonTransition
            {
                Rel = rel,
                Uri = (href == null) ? null : href.Value<string>(),
                Title = (title == null) ? null : title.Value<string>(),
                Type = (type == null) ? null : type.Value<string>(),
                UriIsTemplated = templated,
                DepreciationUri = (deprecated == null) ? null : deprecated.Value<string>(),
                Name = (name == null) ? null : name.Value<string>(),
                ProfileUri = (profile == null) ? null : profile.Value<string>(),
                LanguageTag = (hreflang == null) ? null : hreflang.Value<string>()
            };

            return transition;
        }
    }
}
