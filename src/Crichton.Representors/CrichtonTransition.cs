using System.Collections.Generic;

namespace Crichton.Representors
{
    /// <summary>
    /// TransitionRenderMethod enum
    /// </summary>
    public enum TransitionRenderMethod
    {
        /// <summary>Undefined</summary>
        Undefined = 0,

        /// <summary>Embed</summary>
        Embed = 1,

        /// <summary>Resouce</summary>
        Resource = 2
    }

    /// <summary>
    /// CrichtonTransition class
    /// </summary>
    public class CrichtonTransition : IAttributesContainer
    {
        /// <summary>Gets or sets the Rel</summary>
        public string Rel { get; set; }

        /// <summary>Gets or sets the Uri</summary>
        public string Uri { get; set; }

        /// <summary>Gets or sets the UriIsTemplated</summary>
        public bool UriIsTemplated { get; set; }

        /// <summary>Gets or sets the InterfaceMethod</summary>
        public string InterfaceMethod { get; set; }

        /// <summary>Gets or sets the Title</summary>
        public string Title { get; set; }

        /// <summary>Gets or sets the Type</summary>
        public string Type { get; set; }

        /// <summary>Gets or sets the DepreciationUri</summary>
        public string DepreciationUri { get; set; }

        /// <summary>Gets or sets the Name</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the ProfileUri</summary>
        public string ProfileUri { get; set; }

        /// <summary>Gets or sets the LanguageTag</summary>
        public string LanguageTag { get; set; } // per http://tools.ietf.org/html/rfc5988

        /// <summary>Gets or sets the Methods</summary>
        public string[] Methods { get; set; }

        /// <summary>Gets or sets the MediaTypesAccepted</summary>
        public string[] MediaTypesAccepted { get; set; }

        /// <summary>Gets or sets the RenderMethod</summary>
        public TransitionRenderMethod RenderMethod { get; set; }

        /// <summary>Gets or sets the Target</summary>
        public string Target { get; set; }

        /// <summary>Gets or sets the Attributes</summary>
        public IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }

        /// <summary>Gets or sets the Parameters</summary>
        public IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the CrichtonTransition class.
        /// </summary>
        public CrichtonTransition()
        {
            Attributes = new Dictionary<string, CrichtonTransitionAttribute>();
            Parameters = new Dictionary<string, CrichtonTransitionAttribute>();
        }
    }
}