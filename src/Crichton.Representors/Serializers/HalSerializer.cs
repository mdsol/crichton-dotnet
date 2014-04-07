using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crichton.Representors.Serializers.Hal;
using Newtonsoft.Json;

namespace Crichton.Representors.Serializers
{
    public class HalSerializer
    {
        public string Serialize(CrichtonRepresentor representor)
        {
            var document = new HalDocument();
            
            if (String.IsNullOrWhiteSpace(representor.SelfLink)) throw new InvalidOperationException("HAL Specification requires Self Link to be set.");

            // set Self link
            document.Links = new Dictionary<string, Link> {{"self", new Link(representor.SelfLink)}};

            return JsonConvert.SerializeObject(document);
        }
    }
}
