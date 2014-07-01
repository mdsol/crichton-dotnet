using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    [ContractClass(typeof(IHypermediaQueryContracts))]
    public interface IHypermediaQuery
    {
        IList<IQueryStep> Steps { get; }
        void AddStep(IQueryStep step);
        Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestHandler requestHandler);
        IHypermediaQuery Clone();
    }
}