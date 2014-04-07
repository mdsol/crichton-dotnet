using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Crichton.Representors.Tests.Serializers
{
    public class HalSerializerTests : TestBase, IUseFixture<HalSerializer>
    {
        private HalSerializer sut;

        public void SetFixture(HalSerializer data)
        {
            sut = data;
        }

        [Theory, AutoData]
        public void SelfLinkIsSet(CrichtonRepresentor representor)
        {
            var result = JObject.Parse(sut.Serialize(representor));

            Assert.Equal(result["_links"]["self"].Value<string>("href"), representor.SelfLink);
        }

        [Theory, AutoData]
        public void ThrowsExceptionIfNoSelfLinkSet(CrichtonRepresentor representor)
        {
            representor.SelfLink = null;

            Assert.Throws<InvalidOperationException>(() => sut.Serialize(representor));
        }


    }
}
