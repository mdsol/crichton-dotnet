using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks.Constraints;

namespace Crichton.Representors.Tests.Serializers
{
    public class HalSerializerTests : TestBase
    {
        private HalSerializer sut;
        private CrichtonRepresentor<ExampleDataObject> representor;
        private IFixture fixture;

        [TestFixtureSetUp]
        public void Init()
        {
            sut = new HalSerializer();
            fixture = GetFixture();
            representor = fixture.Create<CrichtonRepresentor<ExampleDataObject>>();
        }

        [Test]
        public void Serialize_SelfLinkIsSet()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            Assert.AreEqual(result["_links"]["self"].Value<string>("href"), representor.SelfLink);
        }

        [Test]
        public void Serialize_ThrowsExceptionIfNoSelfLinkSet()
        {
            representor.SelfLink = null;

            Assert.Throws<InvalidOperationException>(() => sut.Serialize(representor));
        }

        [Test]
        public void Serialize_AddsPropertiesToRootForEachDataObjectProperty()
        {
            var dataJobject = JObject.FromObject(representor.Data);

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var property in dataJobject.Properties())
            {
                Assert.AreEqual(dataJobject.GetValue(property.Name), result.GetValue(property.Name));
            }
        }

        [Test]
        public void Deserialize_SetsSelfLink()
        {
            var href = fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{0}""
                                }}
                }}
            }}";

            json = String.Format(json, href);

            var result = sut.Deserialize<ExampleDataObject>(json);

            Assert.AreEqual(href, result.SelfLink);

        }

        [Test]
        public void Deserialize_DeserializesObjectCorrectly()
        {
            var href = fixture.Create<string>();
            var id = fixture.Create<string>();
            var intId = fixture.Create<int>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{0}""
                                }}
                }},
                ""id"" : ""{1}"",
                ""int_id"" : ""{2}""
            }}";

            json = String.Format(json, href, id, intId);

            var result = sut.Deserialize<ExampleDataObject>(json);

            Assert.AreEqual(id, result.Data.Id);
            Assert.AreEqual(intId, result.Data.IntegerId);
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenNoLinksSet()
        {
            var json = @"{ }";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize<ExampleDataObject>(json));
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenNoSelfLinkSet()
        {
            var json = @"{ ""_links"" : { ""not-self"" : {""href"" : ""not-self"" }}}";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize<ExampleDataObject>(json));
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenHrefInSelfLinkIsBlank()
        {
            var json = @"{ ""_links"" : { ""self"" : {""not-href"" : """" }}}";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize<ExampleDataObject>(json));
        }

    }
}
