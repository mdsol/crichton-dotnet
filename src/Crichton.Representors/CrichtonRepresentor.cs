using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crichton.Representors
{
    public class CrichtonRepresentor<T>
    {
        public string SelfLink { get; set; }

        public T Data { get; set; }
    }
}
