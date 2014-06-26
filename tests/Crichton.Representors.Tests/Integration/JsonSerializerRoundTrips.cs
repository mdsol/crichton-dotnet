using Crichton.Representors.Serializers;
using NUnit.Framework;

namespace Crichton.Representors.Tests.Integration
{
    public class JsonSerializerRoundTrips : RoundTripTests
    {
        private JsonSerializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new JsonSerializer();
            Fixture = GetFixture();
        }

        [Test]
        public void SimpleAttributes_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Json\\SimpleAttributes", serializer);
        }

        [Test]
        public void NestedAttributes_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Json\\NestedAttributes", serializer);
        }

        [Test]
        public void ComplexResources_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Json\\ComplexResources", serializer);
        }

    }
}
