using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private const string DefaultMediaType = "application/vnd.json+sausage";
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
        public async Task RequestTransitionAsync_CallsSendAsyncWithRequestMessageForSimpleTransition()
        {
            const string relativeUri = "api/sausages/1";
            var transition = new CrichtonTransition { Uri = relativeUri };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder))))
                        .Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = DefaultMediaType;

            var result = await sut.RequestTransitionAsync(transition);

            Assert.AreEqual(representorResult, result);
        }

        [Test]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task RequestTransitionAsync_ThrowsHttpExceptionForNonHttpSuccessCode()
        {
            const string relativeUri = "api/sausages/1";
            var transition = new CrichtonTransition { Uri = relativeUri };

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl;
            messageHandler.Response = "oh no";
            messageHandler.ResponseStatusCode = HttpStatusCode.InternalServerError;

            await sut.RequestTransitionAsync(transition);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RequestTransitionAsync_ThrowsInvalidOperationExceptionIfResponseContentTypeDoesNotMatchSerializer()
        {
            const string relativeUri = "api/sausages/1";
            const string mediaType2 = "application/vnd.json+banana";
            var transition = new CrichtonTransition { Uri = relativeUri };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder))))
                        .Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl && m.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue(DefaultMediaType));
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = mediaType2;

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
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(s => s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson), Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder)))).Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Post && m.RequestUri == combinedUrl && m.Content.ReadAsStringAsync().Result == testObjectJson;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = DefaultMediaType;

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
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder)))).Return(representorBuilder);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Put && m.RequestUri == combinedUrl;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = DefaultMediaType;

            var result = await sut.RequestTransitionAsync(transition);

            Assert.AreEqual(representorResult, result);
        }

        [Test]
        public async Task RequestTransitionAsync_AddsAcceptHeaderFromContentTypeOnSerializer()
        {
            const string relativeUri = "api/sausages/1";
            const string mediaType = "application/vnd.json+sausage";
            var transition = new CrichtonTransition { Uri = relativeUri };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder))))
                        .Return(representorBuilder);
            serializer.Stub(s => s.ContentType).Return(mediaType);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl && m.Headers.Accept.Contains(new MediaTypeWithQualityHeaderValue(mediaType));
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = DefaultMediaType;

            var result = await sut.RequestTransitionAsync(transition);

            Assert.AreEqual(representorResult, result);
        }

        [Test]
        public async Task RequestTransitionAsync_UsesAllTransitionRequestFiltersBeforeSendingRequest()
        {
            const string relativeUri = "api/sausages/1";
            var transition = new CrichtonTransition { Uri = relativeUri };
            var representorResult = Fixture.Create<CrichtonRepresentor>();
            var representorAsJson = Fixture.Create<string>();
            var representorBuilder = MockRepository.GenerateMock<IRepresentorBuilder>();
            representorBuilder.Stub(r => r.ToRepresentor()).Return(representorResult);
            serializer.Stub(s => s.ContentType).Return(DefaultMediaType);
            serializer.Stub(
                s =>
                    s.DeserializeToNewBuilder(Arg<string>.Is.Equal(representorAsJson),
                        Arg<Func<IRepresentorBuilder>>.Matches(m => m().GetType() == typeof(RepresentorBuilder))))
                        .Return(representorBuilder);

            var requestFilter = MockRepository.GenerateMock<ITransitionRequestFilter>();
            var requestFilter2 = MockRepository.GenerateMock<ITransitionRequestFilter>();

            sut.AddRequestFilter(requestFilter);
            sut.AddRequestFilter(requestFilter2);

            var combinedUrl = new Uri(baseUri + relativeUri, UriKind.RelativeOrAbsolute);

            messageHandler.Condition = m => m.Method == HttpMethod.Get && m.RequestUri == combinedUrl;
            messageHandler.Response = representorAsJson;
            messageHandler.ResponseStatusCode = HttpStatusCode.OK;
            messageHandler.ContentType = DefaultMediaType;

            await sut.RequestTransitionAsync(transition);

            requestFilter.AssertWasCalled(r => r.Execute(Arg<HttpRequestMessage>.Is.Anything));
            // get the httprequestmessage that was passed to first filter
            var message = (HttpRequestMessage)(requestFilter.GetArgumentsForCallsMadeOn(r => r.Execute(Arg<HttpRequestMessage>.Is.Anything))[0][0]);
            // make sure the second filter was called with the same message as the first
            requestFilter2.AssertWasCalled(r => r.Execute(message));
        }
    }
}
