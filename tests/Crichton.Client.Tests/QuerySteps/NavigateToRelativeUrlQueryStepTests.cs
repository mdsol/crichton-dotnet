using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests.QuerySteps
{
    public class NavigateToRelativeUrlQueryStepTests : TestWithFixture
    {
        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
        }

        [Test]
        public async Task ExecuteAsync_RequestsTransitionWithRelativeUrlAsUrl()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var expected = Fixture.Create<CrichtonRepresentor>();
            var url = Fixture.Create<string>();

            var requestor = MockRepository.GenerateMock<ITransitionRequestHandler>();
            requestor.Stub(r => r.RequestTransitionAsync(Arg<CrichtonTransition>.Matches(t => t.Uri == url), Arg<object>.Is.Null)).Return(Task.FromResult(expected));

            var sut = new NavigateToRelativeUrlQueryStep(url);

            var result = await sut.ExecuteAsync(representor, requestor);

            Assert.AreEqual(expected, result);
        }
    }
}
