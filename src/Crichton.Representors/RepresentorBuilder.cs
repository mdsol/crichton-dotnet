using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            representor.SelfLink = self;
        }

        public void SetAttributes(JObject attributes)
        {
            representor.Attributes = attributes;
        }

        public void SetAttributesFromObject(object data)
        {
            representor.Attributes = JObject.FromObject(data);
        }

        public void AddTransition(string rel, string uri)
        {
            AddTransition(rel, uri, null);
        }

        public void AddTransition(string rel, string uri, string title)
        {
            representor.Transitions.Add(new CrichtonTransition() { Rel = rel, Uri = uri, Title = title });
        }

        public void AddEmbeddedResource(string key, CrichtonRepresentor resource)
        {
            if (!representor.EmbeddedResources.ContainsKey(key))
                representor.EmbeddedResources[key] = new List<CrichtonRepresentor>();

            representor.EmbeddedResources[key].Add(resource);
        }

        public void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc)
        {
            foreach (var item in collection)
            {
                var newRepresentor = new CrichtonRepresentor {SelfLink = selfLinkFunc(item)};
                newRepresentor.SetAttributesFromObject(item);

                representor.Collection.Add(newRepresentor);
            }
        }
    }
}
