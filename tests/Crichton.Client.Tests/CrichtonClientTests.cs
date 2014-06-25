using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using Crichton.Representors.Serializers;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests
{
    public class CrichtonClientTests : TestWithFixture
    {
        private CrichtonClient sut;

        private HttpClient client;
        private FakeHttpMessageHandler messageHandler;
        private Uri baseUri;
        private ISerializer serializer;
        private ITransitionRequestHandler requestHandler;

        [SetUp]
        public void Init()
        {
            messageHandler = new FakeHttpMessageHandler();
            baseUri = new Uri("http://www.my-awesome-company.com");
            client = new HttpClient(messageHandler);
            serializer = MockRepository.GenerateMock<ISerializer>();
            requestHandler = MockRepository.GenerateMock<ITransitionRequestHandler>();

            sut = new CrichtonClient(requestHandler);
            Fixture = GetFixture();
        }

        [Test]
        public void CTOR_SetsTransitionRequestHandler()
        {
            Assert.AreEqual(requestHandler, sut.TransitionRequestHandler);
        }

        [Test]
        public void CTOR_SetsHttpClientTransitionRequestHandlerWhenNotSpecified()
        {
            sut = new CrichtonClient(baseUri, serializer);
            Assert.IsInstanceOf<HttpClientTransitionRequestHandler>(sut.TransitionRequestHandler);
        }

        [Test]
        public void CreateQuery_ReturnsNewInstanceOfHypermediaQuery()
        {
            var result = sut.CreateQuery();

            Assert.IsInstanceOf<HypermediaQuery>(result);
        }

        [Test]
        public async Task ExecuteQueryAsync_ExecutesTheQueryAndReturnsTheResult()
        {
            var query = MockRepository.GenerateMock<IHypermediaQuery>();
            var representor = Fixture.Create<CrichtonRepresentor>();

            query.Stub(q => q.ExecuteAsync(requestHandler)).Return(Task.FromResult(representor));

            var result = await sut.ExecuteQueryAsync(query);

            Assert.AreEqual(representor, result);
        }

        [Test]
        public async Task CreateQuery_SetsFirstStepAsNavigateToRepresentorQueryStep()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var result = sut.CreateQuery(representor);

            var step = (NavigateToRepresentorQueryStep)result.Steps.Single();

            Assert.AreEqual(representor, await step.ExecuteAsync(null,null));
            
        }

    }
}
