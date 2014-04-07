using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers.Hal;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Crichton.Representors.Tests.Serializers.Hal
{
    public class LinkTests : TestBase
    {
        private IFixture fixture;
        private Link sut;

        [TestFixtureSetUp]
        public void Init()
        {
            fixture = GetFixture();
            sut = new Link();
        }

        [Test]
        public void CTOR_Default()
        {
            sut = new Link();
        }

        [Test]
        public void CTOR_WithHrefSetsHref()
        {
            var href = fixture.Create<string>();
            sut = new Link(href);
            Assert.AreEqual(href, sut.Href);
        }
    }
}
