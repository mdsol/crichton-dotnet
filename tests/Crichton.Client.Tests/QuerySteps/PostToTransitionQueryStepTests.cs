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

namespace Crichton.Client.Tests.QuerySteps
{
    public class PostToTransitionQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        public async Task ExecuteAsync_PostsDataToRequestor()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var expected = Fixture.Create<CrichtonRepresentor>();
            var data = new {id = 1, name = "123"};
            var transition = representor.Transitions.First();
            var rel = transition.Rel;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(transition, data)).Return(Task.FromResult(expected));

            var sut = new PostToTransitionQueryStep(rel, data);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public async Task ExecuteAsync_PostsDataToRequestorUsingTransitionRequestorFromFunction()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var expected = Fixture.Create<CrichtonRepresentor>();
            var data = new { id = 1, name = "123" };
            var transition = representor.Transitions.First();
            var rel = Fixture.Create<string>();
            var name = Fixture.Create<string>();
            transition.Rel = rel;
            transition.Name = name;

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(transition, data)).Return(Task.FromResult(expected));

            var sut = new PostToTransitionQueryStep(t => t.Rel == rel && t.Name == name, data);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }
    }
}
