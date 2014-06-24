using System.Collections.Generic;
using System.Threading.Tasks;
using Crichton.Representors;

namespace Crichton.Client
{
    public interface IHypermediaQuery
    {
        IList<IQueryStep> Steps { get; }
        void AddStep(IQueryStep step);
        Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestor requestor);
    }
}