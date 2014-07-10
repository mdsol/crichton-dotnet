using System.Collections.Generic;

namespace Crichton.Representors
{
    /// <summary>
    /// IAttributesContainer interface
    /// </summary>
    public interface IAttributesContainer
    {
        /// <summary>
        /// Gets or sets the Attributes
        /// </summary>
        IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the Parameters
        /// </summary>
        IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }
    }
}