using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Crichton.Representors.Tests.Serializers
{
    public class HalSerializerTests : TestBase
    {
        private HalSerializer sut;
        private CrichtonRepresentor representor;
        private IFixture fixture;

        [TestFixtureSetUp]
        public void Init()
        {
            sut = new HalSerializer();
            fixture = GetFixture();
            representor = fixture.Create<CrichtonRepresentor>();
        }

        [Test]
        public void SelfLinkIsSet()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            Assert.AreEqual(result["_links"]["self"].Value<string>("href"), representor.SelfLink);
        }

        [Test]
        public void ThrowsExceptionIfNoSelfLinkSet()
        {
            representor.SelfLink = null;

            Assert.Throws<InvalidOperationException>(() => sut.Serialize(representor));
        }

    }
}
