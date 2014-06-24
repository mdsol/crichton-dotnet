using System;
using System.Net.Http;
using System.Threading.Tasks;
using Crichton.Representors;
using Crichton.Representors.Serializers;
using NUnit.Framework;
using Ploeh.AutoFixture;
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

    }
}
