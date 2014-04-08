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
    public class HalSerializerTests : TestWithFixture
    {
        private HalSerializer sut;
        private CrichtonRepresentor representor;

        [TestFixtureSetUp]
        public void Init()
        {
            sut = new HalSerializer();
            Fixture = GetFixture();
            representor = Fixture.Create<CrichtonRepresentor>();
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
        public void Serialize_AddsPropertiesToRootForEachAttributeInRepresentor()
        {
            var dataJobject = JObject.FromObject(Fixture.Create<ExampleDataObject>());
            representor.Attributes = dataJobject;

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var property in dataJobject.Properties())
            {
                Assert.AreEqual(dataJobject.GetValue(property.Name), result.GetValue(property.Name));
            }
        }

        [Test]
        public void Deserialize_SetsSelfLink()
        {
            var href = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{0}""
                                }}
                }}
            }}";

            json = String.Format(json, href);

            var result = sut.Deserialize(json);

            Assert.AreEqual(href, result.SelfLink);

        }

        [Test]
        public void Deserialize_DeserializesObjectCorrectly()
        {
            var href = Fixture.Create<string>();
            var id = Fixture.Create<string>();
            var intId = Fixture.Create<int>();
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

            var result = sut.Deserialize(json);

            Assert.AreEqual(id, result.Attributes["id"].Value<string>());
            Assert.AreEqual(intId, result.Attributes["int_id"].Value<int>());
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenNoLinksSet()
        {
            const string json = @"{ }";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize(json));
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenNoSelfLinkSet()
        {
            const string json = @"{ ""_links"" : { ""not-self"" : {""href"" : ""not-self"" }}}";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize(json));
        }

        [Test]
        public void Deserialize_ThrowsExceptionWhenHrefInSelfLinkIsBlank()
        {
            const string json = @"{ ""_links"" : { ""self"" : {""not-href"" : """" }}}";

            Assert.Throws<InvalidOperationException>(() => sut.Deserialize(json));
        }
    }
}
