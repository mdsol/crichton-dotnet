using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class RepresentorBuilder : IRepresentorBuilder
    {
        private readonly CrichtonRepresentor representor;

        public RepresentorBuilder()
        {
            representor = new CrichtonRepresentor();
        }

        public CrichtonRepresentor ToRepresentor()
        {
            return representor;
        }

        public void SetSelfLink(string self)
        {
            if (string.IsNullOrWhiteSpace(self))
            {
                throw new ArgumentNullException("self");
            }

            representor.SelfLink = self;
        }

        public void SetAttributes(JObject attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }

            representor.Attributes = attributes;
        }

        public void SetAttributesFromObject(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            representor.Attributes = JObject.FromObject(data);
        }

        public void AddTransition(CrichtonTransition transition)
        {
            if (transition == null)
            {
                throw new ArgumentNullException("transition");
            }

            representor.Transitions.Add(transition);
        }

        public void AddTransition(string rel, string uri = null, string title = null, string type = null, bool uriIsTemplated = false, 
            string depreciationUri = null, string name = null, string profileUri = null, string languageTag = null)
        {
            if (string.IsNullOrWhiteSpace(rel))
            {
                throw new ArgumentNullException("rel");
            }

            representor.Transitions.Add(new CrichtonTransition { Rel = rel, Uri = uri, Title = title, Type = type, 
                UriIsTemplated = uriIsTemplated, DepreciationUri = depreciationUri, Name = name, ProfileUri = profileUri, LanguageTag = languageTag});
        }

        public void AddEmbeddedResource(string key, CrichtonRepresentor resource)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("key");
            }

            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            if (!representor.EmbeddedResources.ContainsKey(key))
                representor.EmbeddedResources[key] = new List<CrichtonRepresentor>();

            representor.EmbeddedResources[key].Add(resource);
        }

        public void SetCollection(IEnumerable<CrichtonRepresentor> representors)
        {
            if (representors == null)
            {
                throw new ArgumentNullException("representors");
            }

            foreach(var representorInCollection in representors)
                representor.Collection.Add(representorInCollection);
        }

        public void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (selfLinkFunc == null)
            {
                throw new ArgumentNullException("selfLinkFunc");
            }

            foreach (var item in collection)
            {
                var newRepresentor = new CrichtonRepresentor {SelfLink = selfLinkFunc(item)};
                newRepresentor.SetAttributesFromObject(item);

                representor.Collection.Add(newRepresentor);
            }
        }
    }
}
