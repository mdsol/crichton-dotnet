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

        [SetUp]
        public void Init()
        {
            messageHandler = new FakeHttpMessageHandler();
            baseUri = new Uri("http://www.my-awesome-company.com");
            client = new HttpClient(messageHandler);
            client.BaseAddress = baseUri;
            serializer = MockRepository.GenerateMock<ISerializer>();

            sut = new CrichtonClient(client, baseUri, serializer);
            Fixture = GetFixture();
        }

        [Test]
        public void CTOR_SetsHttpClient()
        {
            Assert.AreEqual(client, sut.HttpClient);
        }

        [Test]
        public void CTOR_SetsBaseUri()
        {
            Assert.AreEqual(baseUri, sut.BaseUri);
        }

        [Test]
        public void CTOR_SetsSerializer()
        {
            Assert.AreEqual(serializer, sut.Serializer);
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

            query.Stub(q => q.ExecuteAsync(sut)).Return(Task.FromResult(representor));

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

        [Test]
        public async Task RequestTransitionAsync_CallsSendAsyncWithRequestMessageForSimpleTransition()
        {
            const string relativeUri = "api/sausages/1";
            var transition = new CrichtonTransition {Uri = relativeUri};
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Is.Anything)).IgnoreArguments().Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;

            var result = await sut.RequestTransitionAsync(transition);

            Assert.AreEqual(representorResult, result);
        }

        [Test]
        public async Task PostTransitionDataAsJsonAsync_PostsJsonRepresentationOfObject()
        {
            var testObject = new {id = 2, name = "bratwurst"};
            var testObjectJson = JsonConvert.SerializeObject(testObject);

            const string relativeUri = "api/sausages";
            var transition = new CrichtonTransition { Uri = relativeUri };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(s => s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson), Arg<Func<IRepresentorBuilder>>.Is.Anything)).IgnoreArguments().Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Post && m.RequestUri == combinedUrl && m.Content.ReadAsStringAsync().Result == testObjectJson;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;

            var result = await sut.PostTransitionDataAsJsonAsync(transition, testObject);

            Assert.AreEqual(representorResult, result);
        }

    }
}
