using System.Collections.Generic;

namespace Crichton.Representors
{
    public interface IAttributesContainer
    {
        IDictionary<string, CrichtonTransitionAttribute> Attributes { get; set; }
        IDictionary<string, CrichtonTransitionAttribute> Parameters { get; set; }
    }
}