using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;

namespace Crichton.Representors.Tests.Serializers
{
    public class HalSerializerTests : TestBase
    {
        private HalSerializer sut;

        [TestFixtureSetUp]
        public void Init()
        {
            sut = new HalSerializer();
        }

        [Test, AutoData]
        public void SelfLinkIsSet(CrichtonRepresentor representor)
        {
            var result = JObject.Parse(sut.Serialize(representor));

            Assert.AreEqual(result["_links"]["self"].Value<string>("href"), representor.SelfLink);
        }

        [Test, AutoData]
        public void ThrowsExceptionIfNoSelfLinkSet(CrichtonRepresentor representor)
        {
            representor.SelfLink = null;

            Assert.Throws<InvalidOperationException>(() => sut.Serialize(representor));
        }


    }
}
