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

        private HypermediaQuery(IEnumerable<IQueryStep> steps)
        {
            Steps = steps.ToList();
        }

        public void AddStep(IQueryStep step)
        {
            Steps.Add(step);
        }

        public async Task<CrichtonRepresentor> ExecuteAsync(ITransitionRequestHandler requestHandler)
        {
            CrichtonRepresentor representor = null;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var step in Steps)
            {
                representor = await step.ExecuteAsync(representor, requestHandler);
            }

            return representor;
        }

        public IHypermediaQuery Clone()
        {
            return new HypermediaQuery(this.Steps);
        }
    }
}
