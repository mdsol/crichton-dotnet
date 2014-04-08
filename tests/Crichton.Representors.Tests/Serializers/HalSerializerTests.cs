using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using FluentAssertions;
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

        [SetUp]
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
        public void Serialize_AddsHrefLinkAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Uri, result["_links"][transition.Rel]["href"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsMultipleLinkItemsWhenTransitionRelIsShared()
        {
            var fixedRel = Fixture.Create<string>();
            foreach (var transition in representor.Transitions) transition.Rel = fixedRel;

            var result = JObject.Parse(sut.Serialize(representor));

            var links = result["_links"];
            var relArray = links[fixedRel];
            foreach (var transition in representor.Transitions)
            {
                Assert.IsTrue(relArray.Any(l => l.Value<string>("href") == transition.Uri));
            }
        }

        [Test]
        public void Serialize_DoesNotThrowForNullSelfLink()
        {
            representor.SelfLink = null;
            Assert.DoesNotThrow(() => sut.Serialize(representor));
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
        public void Deserialize_SetsTransitionsForSingleLink()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"" }}
                }}
            }}";

            json = String.Format(json, rel, href);

            var result = sut.Deserialize(json);

            result.Transitions.Should().Contain(t => t.Rel == rel && t.Uri == href);
        }

        [Test]
        public void Deserialize_SetsTransitionsForMultipleLinks()
        {
            var href = Fixture.Create<string>();
            var href2 = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": [ {{ ""href"" : ""{1}"" }}, {{ ""href"" : ""{2}"" }} ]
                }}
            }}";

            json = String.Format(json, rel, href, href2);

            var result = sut.Deserialize(json);

            result.Transitions.Should().Contain(t => t.Rel == rel && t.Uri == href);
            result.Transitions.Should().Contain(t => t.Rel == rel && t.Uri == href2);
        }

        [Test]
        public void Deserialize_DoesNotThrowForEmptyDocument()
        {
            const string json = @"{ }";

            Assert.DoesNotThrow(() => sut.Deserialize(json));
        }

    }
}
