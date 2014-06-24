using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;

namespace Crichton.Client
{
    public class HypermediaQuery : IHypermediaQuery
    {
        public IList<IQueryStep> Steps { get; private set; }

        public HypermediaQuery()
        {
            Steps = new List<IQueryStep>();
        }

        public void AddStep(IQueryStep step)
        {
            Steps.Add(step);
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestor requestor)
        {
            CrichtonRepresentor representor = null;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var step in Steps)
            {
                representor = await step.ExecuteAsync(representor, requestor);
            }

            return representor;
        }
    }
}
