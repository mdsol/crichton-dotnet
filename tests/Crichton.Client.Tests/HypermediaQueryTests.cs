using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests
{
    public class HypermediaQueryTests : TestWithFixture
    {
        private IHypermediaQuery sut;

        [SetUp]
        public void Init()
        {
            sut = new HypermediaQuery();
            Fixture = GetFixture();
        }

        [Test]
        public void CTOR_CreatesNewStepsList()
        {
            Assert.IsNotNull(sut.Steps);
        }

        [Test]
        public void AddStep_AddsStepToList()
        {
            var step = Fixture.Create<IQueryStep>();

            sut.AddStep(step);

            CollectionAssert.Contains(sut.Steps, step);
        }

        [Test]
        public async Task ExecuteAsync_ReturnsResultOfChainedCallsToSteps()
        {
            var requestor = Fixture.Create<ITransitionRequestHandler>();
            var step1 = MockRepository.GenerateMock<IQueryStep>();
            var step1Result = Fixture.Create<CrichtonRepresentor>();
            step1.Stub(s => s.ExecuteAsync(null, requestor)).Return(Task.FromResult(step1Result));

            var step2 = MockRepository.GenerateMock<IQueryStep>();
            var step2Result = Fixture.Create<CrichtonRepresentor>();
            step2.Stub(s => s.ExecuteAsync(step1Result, requestor)).Return(Task.FromResult(step2Result));

            sut.Steps.Add(step1);
            sut.Steps.Add(step2);

            var result = await sut.ExecuteAsync(requestor);

            Assert.AreEqual(step2Result, result);
        }
    }
}
