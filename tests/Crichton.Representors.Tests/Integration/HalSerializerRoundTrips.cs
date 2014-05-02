using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Integration
{
    public class HalSerializerRoundTrips : RoundTripTests
    {
        private HalSerializer serializer;

        [SetUp]
        public void Init()
        {
            serializer = new HalSerializer();
            Fixture = GetFixture();
        }

        [Test]
        public void SelfLinkOnly_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\SelfLinkOnly", serializer);
        }

        [Test]
        public void MultipleLinksSameRelation_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\MultipleLinksSameRelation", serializer);
        }

        [Test]
        public void SimpleLinksAndAttributes_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\SimpleLinksAndAttributes", serializer);
        }

        // From "Resources" here: https://phlyrestfully.readthedocs.org/en/latest/halprimer.html
        [Test]
        public void ComplexEmbeddedResources_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\ComplexEmbeddedResources", serializer);
        }

        [Test]
        public void WormholeSample_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\WormholeSample", serializer);
        }

        [Test]
        public void HalAllLinkObjectProperties_RoundTrip()
        {
            TestRoundTripFromJsonTestData("Hal\\HalAllLinkObjectProperties", serializer);
        }

    }
}
