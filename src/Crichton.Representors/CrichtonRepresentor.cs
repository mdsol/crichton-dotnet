using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
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

    public class CrichtonTransition
    {
        public string Rel { get; set; }
        public string Uri { get; set; }
        public string TemplatedUri { get; set; }
        public string InterfaceMethod { get; set; }
        public IList<CrichtonTransitionAttribute> Attributes { get; set; } 
    }

    public class CrichtonTransitionAttribute
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Type DataType { get; set; }
        public string Constraints { get; set; }
        public IList<string> Options { get; set; } 
    }
}
