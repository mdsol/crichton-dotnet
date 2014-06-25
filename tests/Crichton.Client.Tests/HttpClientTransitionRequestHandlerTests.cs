using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors;
using Crichton.Representors.Serializers;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests
{
    public class HttpClientTransitionRequestHandlerTests : TestWithFixture
    {
        private HttpClientTransitionRequestHandler sut;

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

            sut = new HttpClientTransitionRequestHandler(client, serializer);
            Fixture = GetFixture();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CTOR_ThrowsInvalidOperationExceptionWhenBaseAddressIsNotSetInHttpClient()
        {
            client.BaseAddress = null;
            sut = new HttpClientTransitionRequestHandler(client, serializer);
        }

        [Test]
        public async Task RequestTransitionAsync_CallsSendAsyncWithRequestMessageForSimpleTransition()
        {
            const string relativeUri = "api/sausages/1";
            var transition = new CrichtonTransition { Uri = relativeUri };
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
            var testObject = new { id = 2, name = "bratwurst" };
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

            var result = await sut.RequestTransitionAsync(transition, testObject);

            Assert.AreEqual(representorResult, result);
        }

        [Test]
        public async Task RequestTransitionAsync_SupportsCustomHttpMethodInTransition()
        {
            const string relativeUri = "api/sausages/1";
            const string httpMethod = "Put";
            var transition = new CrichtonTransition { Uri = relativeUri, Methods = new []{httpMethod} };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Is.Anything)).IgnoreArguments().Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Put && m.RequestUri == combinedUrl;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;

            var result = await sut.RequestTransitionAsync(transition);

            Assert.AreEqual(representorResult, result);
        }
    }
}
