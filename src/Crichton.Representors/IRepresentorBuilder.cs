using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public interface IRepresentorBuilder
    {
        CrichtonRepresentor ToRepresentor();
        void SetSelfLink(string self);
        void SetAttributes(JObject attributes);
        void SetAttributesFromObject(object data);
        void AddTransition(string rel, string uri = null, string title = null, string type = null,
            bool uriIsTemplated = false, string depreciationUri = null);
        void AddEmbeddedResource(string key, CrichtonRepresentor resource);
        void SetCollection(IEnumerable<CrichtonRepresentor> representors);
        void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc);
    }
}