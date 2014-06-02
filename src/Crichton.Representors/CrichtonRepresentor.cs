﻿using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class CrichtonRepresentor
    {
        public string SelfLink { get; set; }

        public JObject Attributes { get; set; }

        public IList<CrichtonTransition> Transitions { get; set; }

        public Dictionary<string, IList<CrichtonRepresentor>> EmbeddedResources { get; private set; }

        public ICollection<CrichtonRepresentor> Collection { get; private set; } 

        public CrichtonRepresentor()
        {
            Transitions = new List<CrichtonTransition>();
            EmbeddedResources = new Dictionary<string, IList<CrichtonRepresentor>>();
            Collection = new List<CrichtonRepresentor>();
            Attributes = new JObject();
        }

        public T ToObject<T>()
        {
            return Attributes.ToObject<T>();
        }

        public void SetAttributesFromObject(object data)
        {
            Attributes = JObject.FromObject(data);
        }
    }
}
