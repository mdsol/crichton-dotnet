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
