using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class CrichtonRepresentor
    {
        public string SelfLink { get; set; }

        public JObject Attributes { get; set; }

        public IList<CrichtonTransition> Transitions { get; set; }

        public CrichtonRepresentor()
        {
            Transitions = new List<CrichtonTransition>();
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
