using System.Collections.Generic;

namespace Crichton.Representors
{
    /// <summary>
    /// CrichtonTransitionAttributeConstraint class
    /// </summary>
    public class CrichtonTransitionAttributeConstraint
    {
        /// <summary>Gets or sets the Option</summary>
        public IList<string> Options { get; set; }

        /// <summary>Gets or sets the IsIne</summary>
        public bool? IsIn { get; set; }

        /// <summary>Gets or sets the Min</summary>
        public int? Min { get; set; }

        /// <summary>Gets or sets the MinLength</summary>
        public int? MinLength { get; set; }

        /// <summary>Gets or sets the Max</summary>
        public int? Max { get; set; }

        /// <summary>Gets or sets the MaxLength</summary>
        public int? MaxLength { get; set; }

        /// <summary>Gets or sets the Pattern</summary>
        public string Pattern { get; set; }

        /// <summary>Gets or sets the IsMulti</summary>
        public bool? IsMulti { get; set; }

        /// <summary>Gets or sets the IsRequired</summary>
        public bool? IsRequired { get; set; }
    }
}