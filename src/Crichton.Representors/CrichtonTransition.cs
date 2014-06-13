using System.Collections.Generic;

namespace Crichton.Representors
{
    public enum TransitionRenderMethod
    {
        Undefined = 0,
        Embed = 1,
        Resource = 2
    }

    public class CrichtonTransition : IAttributesContainer
    {
        public string Rel { get; set; }
        public string Uri { get; set; }
        public bool UriIsTemplated { get; set; }
        public string InterfaceMethod { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string DepreciationUri { get; set; }
        public string Name { get; set; }
        public string ProfileUri { get; set; }
        public string LanguageTag { get; set; } // per http://tools.ietf.org/html/rfc5988
        public string[] Methods { get; set; }
        public string[] MediaTypesAccepted { get; set; }
        public TransitionRenderMethod RenderMethod { get; set; }
        public string Target { get; set; }
        public IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }
        public IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }

        public CrichtonTransition()
        {
            Attributes = new Dictionary<string, CrichtonTransitionAttribute>();
            Parameters = new Dictionary<string, CrichtonTransitionAttribute>();
        }
    }
}