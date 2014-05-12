using System;
using System.Collections.Generic;

namespace Crichton.Representors
{
    public class CrichtonTransitionAttribute
    {
        public string Default { get; set; }
        public string ProfileUri { get; set; }
        public string JsonType { get; set; }
        public string DataType { get; set; }
        public string Constraints { get; set; }
        public IList<string> Options { get; set; } 
    }
}