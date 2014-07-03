using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests.QuerySteps
{
    public class NavigateToSelfLinkQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        public async Task ExecuteAsync_RequestsTransitionWithSelfLinkAsUrl()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var expected = Fixture.Create<CrichtonRepresentor>();

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(Arg<CrichtonTransition>.Matches(t => t.Uri == representor.SelfLink), Arg<object>.Is.Null)).Return(Task.FromResult(expected));

            var sut = new NavigateToSelfLinkQueryStep();

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }
    }
}
