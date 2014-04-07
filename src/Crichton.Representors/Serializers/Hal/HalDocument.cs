using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Crichton.Representors.Serializers.Hal
{
    public class HalDocument
    {
        [JsonProperty("_links")]
        public IDictionary<string, Link> Links { get; set; }

    }
}
