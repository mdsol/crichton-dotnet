using System.Collections.Generic;

namespace Crichton.Representors
{
    /// <summary>
    /// CrichtonTransitionAttribute class
    /// </summary>
    public class CrichtonTransitionAttribute : IAttributesContainer
    {
        /// <summary>Gets or sets the Value</summary>
        public object Value { get; set; }

        /// <summary>Gets or sets the ProfileUri</summary>
        public string ProfileUri { get; set; }

        /// <summary>Gets or sets the JsonType</summary>
        public string JsonType { get; set; }

        /// <summary>Gets or sets the DataType</summary>
        public string DataType { get; set; }

        /// <summary>Gets or sets the Attributes</summary>
        public IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }

        /// <summary>Gets or sets the Parameters</summary>
        public IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }

        /// <summary>Gets or sets the Constraint</summary>
        public CrichtonTransitionAttributeConstraint Constraint { get; set; }

        /// <summary>
        /// Initializes a new instance of the CrichtonTransitionAttribute class.
        /// </summary>
        public CrichtonTransitionAttribute()
        {
            Attributes = new Dictionary<string, CrichtonTransitionAttribute>();
            Parameters = new Dictionary<string, CrichtonTransitionAttribute>();
        }
    }
}