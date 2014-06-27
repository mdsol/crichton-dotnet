using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Crichton.Client
{
    public interface ITransitionRequestFilter
    {
        void Execute(HttpRequestMessage httpRequestMessage);
    }
}
