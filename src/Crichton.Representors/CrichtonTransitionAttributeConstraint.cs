using System.Collections.Generic;

namespace Crichton.Representors
{
    public class CrichtonTransitionAttributeConstraint
    {
        public IList<string> Options { get; set; }
        public bool? IsIn { get; set; }
        public int? Min { get; set; }
        public int? MinLength { get; set; }
        public int? Max { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
        public bool? IsMulti { get; set; }
        public bool? IsRequired { get; set; }
    }
}