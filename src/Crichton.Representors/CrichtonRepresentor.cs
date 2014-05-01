using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class CrichtonRepresentor
    {
        public string SelfLink { get; set; }

        public JObject Attributes { get; set; }

        public IList<CrichtonTransition> Transitions { get; set; }

        public Dictionary<string, IList<CrichtonRepresentor>> EmbeddedResources { get; private set; }

        public IList<CrichtonRepresentor> Collection { get; private set; } 

        public CrichtonRepresentor()
        {
            Transitions = new List<CrichtonTransition>();
            EmbeddedResources = new Dictionary<string, IList<CrichtonRepresentor>>();
            Collection = new List<CrichtonRepresentor>();
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
