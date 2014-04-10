using System;
using Crichton.Representors.Serializers;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Serializers
{
    public class HalSerializerTests : TestWithFixture
    {
        private HalSerializer sut;
        private CrichtonRepresentor representor;
        private IRepresentorBuilder builder;

        [SetUp]
        public void Init()
        {
            sut = new HalSerializer();
            Fixture = GetFixture();
            representor = Fixture.Create<CrichtonRepresentor>();
            builder = MockRepository.GenerateMock<IRepresentorBuilder>();
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
        public void Serialize_AddsTitleLinkAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Title, result["_links"][transition.Rel]["title"].Value<string>());
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
                var crichtonTransition = transition; // copy to prevent "Access to foreach variable in closure" warning
                relArray.Should().Contain(l => l.Value<string>("href") == crichtonTransition.Uri);
            }
        }

        [Test]
        public void Serialize_DoesNotThrowForNullSelfLink()
        {
            representor.SelfLink = null;
            Assert.DoesNotThrow(() => sut.Serialize(representor));
        }

        [Test]
        public void DeserializeToBuilder_SetsSelfLinkOnBuilder()
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

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasCalled(b => b.SetSelfLink(href));
        }

        [Test]
        public void DeserializeToBuilder_DoesNotSetSelfLinkForPartiallyCompleteLinks()
        {
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""not-href"": ""blah""
                                }}
                }}
            }}";

            json = String.Format(json);

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasNotCalled(b => b.SetSelfLink(Arg<string>.Is.Anything));
        }

        [Test]
        public void DeserializeToBuilder_DoesNotSetSelfLinkForMissingSelf()
        {
            var json = @"
            {{
                ""_links"": {{
                    ""not-self"": {{
                        ""href"": ""blah""
                                }}
                }}
            }}";

            json = String.Format(json);

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasNotCalled(b => b.SetSelfLink(Arg<string>.Is.Anything));
        }

        [Test]
        public void DeserializeToBuilder_DeserializesObjectCorrectly()
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

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasCalled(b => b.SetAttributes(Arg<JObject>.Matches(j => j["id"].Value<string>() == id && j["int_id"].Value<int>() == intId)));

        }

        [Test]
        public void DeserializeToBuilder_SetsTransitionsForSingleLink()
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

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasCalled(b => b.AddTransition(rel, href, null));
        }

        [Test]
        public void DeserializeToBuilder_SetsTransitionsForMultipleLinks()
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

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasCalled(b => b.AddTransition(rel, href, null));
            builder.AssertWasCalled(b => b.AddTransition(rel, href2, null));
        }

        [Test]
        public void DeserializeToBuilder_SetsTransitionsIncludingTitle()
        {
            var href = Fixture.Create<string>();
            var title = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""title"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, title);

            sut.DeserializeToBuilder(json, builder);

            builder.AssertWasCalled(b => b.AddTransition(rel, href, title));
        }

        [Test]
        public void DeserializeToBuilder_DoesNotThrowForEmptyDocument()
        {
            const string json = @"{ }";

            Assert.DoesNotThrow(() => sut.DeserializeToBuilder(json, builder));
        }

    }
}
