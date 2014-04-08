using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Crichton.Representors.Tests
{
    public class ExampleDataObject
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("int_id")]
        public int IntegerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }
    }
}
