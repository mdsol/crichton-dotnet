using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    public interface ITransitionRequestor
    {
        Task<CrichtonRepresentor> RequestTransitionAsync(CrichtonTransition transition);

        Task<CrichtonRepresentor> PostTransitionDataAsJsonAsync(CrichtonTransition transition, object toSerializeToJson);
    }
}
