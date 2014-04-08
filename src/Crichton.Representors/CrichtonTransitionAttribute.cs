using System;
using System.Collections.Generic;

namespace Crichton.Representors
{
    public class CrichtonTransitionAttribute
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Type DataType { get; set; }
        public string Constraints { get; set; }
        public IList<string> Options { get; set; } 
    }
}