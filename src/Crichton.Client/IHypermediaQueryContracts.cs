using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    [ContractClassFor(typeof(IHypermediaQuery))]
    internal abstract class IHypermediaQueryContracts : IHypermediaQuery
    {
        public IList<IQueryStep> Steps
        {
            get
            {
                Contract.Ensures(Contract.Result<IList<IQueryStep>>() != null, "Return value must not be null.");
                return default(IList<IQueryStep>);
            }
        }

        public void AddStep(IQueryStep step)
        {
            Contract.Requires<ArgumentNullException>(step != null, "step must not be null.");
        }

        public Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestHandler requestHandler)
        {
            Contract.Requires<ArgumentNullException>(requestHandler != null, "requestHandler must not be null.");
            Contract.Ensures(Contract.Result<Task<CrichtonRepresentor>>() != null, "Return value must not be null.");

            return default(Task<CrichtonRepresentor>);
        }

        public IHypermediaQuery Clone()
        {
            Contract.Ensures(Contract.Result<IHypermediaQuery>() != null, "Return value must not be null.");

            return default(IHypermediaQuery);
        }
    }
}