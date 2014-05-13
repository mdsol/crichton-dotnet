using System.Collections.Generic;

namespace Crichton.Representors
{
    public class CrichtonTransitionAttribute : IAttributesContainer
    {
        public object Value { get; set; }
        public string ProfileUri { get; set; }
        public string JsonType { get; set; }
        public string DataType { get; set; }
        public IList<string> Options { get; set; }
        public IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }
        public IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }

        public CrichtonTransitionAttribute()
        {
            Attributes = new Dictionary<string, CrichtonTransitionAttribute>();
            Parameters = new Dictionary<string, CrichtonTransitionAttribute>();
        }
    }
}