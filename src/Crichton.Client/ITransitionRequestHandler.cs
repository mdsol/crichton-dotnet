using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    public interface ITransitionRequestHandler
    {
        void AddRequestFilter(ITransitionRequestFilter filter);
        Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition, object toSerializeToJson = null);
    }
}
