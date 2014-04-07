using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Crichton.Representors.Serializers.Hal
{
    public class Link
    {
        [JsonProperty("href")]
        public string Href { get; set; }

        public Link()
        {
            
        }

        public Link(string href)
        {
            Href = href;
        }
    }
}
