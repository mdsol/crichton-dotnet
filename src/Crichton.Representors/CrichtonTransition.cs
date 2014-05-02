using System.Collections.Generic;

namespace Crichton.Representors
{
    public class CrichtonTransition
    {
        public string Rel { get; set; }
        public string Uri { get; set; }
        public bool UriIsTemplated { get; set; }
        public string InterfaceMethod { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string DepreciationUri { get; set; }
        public string Name { get; set; }
        public IList<CrichtonTransitionAttribute> Attributes { get; set; } 
    }
}