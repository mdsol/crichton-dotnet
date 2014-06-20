using System;
using System.Net.Http;
using Crichton.Representors.Serializers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Crichton.Client.Tests
{
    public class CrichtonClientTests : TestWithFixture
    {
        private CrichtonClient sut;

        private HttpClient client;
        private Uri baseUri;
        private ISerializer serializer;

        [SetUp]
        public void Init()
        {
            client = MockRepository.GeneratePartialMock<HttpClient>();
            baseUri = new Uri("http://www.my-awesome-company.com");
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
    }
}
