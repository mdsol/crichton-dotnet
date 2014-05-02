using System;
using System.Collections.Generic;
using System.Linq;
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
        private Func<IRepresentorBuilder> builderFactoryMethod;
            
        [SetUp]
        public void Init()
        {
            sut = new HalSerializer();
            Fixture = GetFixture();
            representor = Fixture.Create<CrichtonRepresentor>();
            builderFactoryMethod = () => MockRepository.GenerateMock<IRepresentorBuilder>();
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
        public void Serialize_AddsTypeLinkAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Type, result["_links"][transition.Rel]["type"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsDepreciationUriAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.DepreciationUri, result["_links"][transition.Rel]["deprecation"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsNameAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Name, result["_links"][transition.Rel]["name"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsProfileUriAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.ProfileUri, result["_links"][transition.Rel]["profile"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsLanguageTagAsHreflangAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.LanguageTag, result["_links"][transition.Rel]["hreflang"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsUriIsTemplatedAttributeForEachTransition()
        {
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                var templatedToken = result["_links"][transition.Rel]["templated"];

                if (templatedToken != null)
                {
                    Assert.AreEqual(transition.UriIsTemplated, templatedToken.Value<bool>());
                }
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
        public void Serialize_SetsSingleEmbeddedResource()
        {
            var key = Fixture.Create<string>();
            var resource = Fixture.Create<CrichtonRepresentor>();

            representor.EmbeddedResources[key] = new List<CrichtonRepresentor> { resource };

            var result = JObject.Parse(sut.Serialize(representor));

            Assert.AreEqual(resource.SelfLink, result["_embedded"][key]["_links"]["self"]["href"].Value<string>());
        }

        [Test]
        public void Serialize_SetsMultipleEmbeddedResourcesAsArray()
        {
            var key = Fixture.Create<string>();
            var resources = Fixture.Create<IList<CrichtonRepresentor>>();

            representor.EmbeddedResources[key] = resources;

            var result = JObject.Parse(sut.Serialize(representor));

            var array = (JArray)result["_embedded"][key];
            foreach (var resource in resources)
            {
                var crichtonRepresentor = resource;
                array.Should().Contain(a => a["_links"]["self"]["href"].Value<string>() == crichtonRepresentor.SelfLink);
            }
        }

        [Test]
        public void Serialize_SetsCollectionToItemsEmbeddedResource()
        {
            representor.Collection.AddMany(() => Fixture.Create<CrichtonRepresentor>(), Fixture.Create<int>());

            var result = JObject.Parse(sut.Serialize(representor));

            var embeddedCollection = (JArray) result["_embedded"]["items"];
            foreach (var resource in representor.Collection)
            {
                var crichtonRepresentor = resource;
                embeddedCollection.Should().Contain(a => a["_links"]["self"]["href"].Value<string>() == crichtonRepresentor.SelfLink);
            }
        }

        [Test]
        public void DeserializeToNewBuilder_SetsSelfLinkOnBuilder()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.SetSelfLink(href));
        }

        [Test]
        public void DeserializeToNewBuilder_DoesNotSetSelfLinkForPartiallyCompleteLinks()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasNotCalled(b => b.SetSelfLink(Arg<string>.Is.Anything));
        }

        [Test]
        public void DeserializeToNewBuilder_DoesNotSetSelfLinkForMissingSelf()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasNotCalled(b => b.SetSelfLink(Arg<string>.Is.Anything));
        }

        [Test]
        public void DeserializeToNewBuilder_DeserializesObjectCorrectly()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.SetAttributes(Arg<JObject>.Matches(j => j["id"].Value<string>() == id && j["int_id"].Value<int>() == intId)));

        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsForSingleLink()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsForMultipleLinks()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href)));
            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href2)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingTitle()
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

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Title == title)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingDeprecated()
        {
            var href = Fixture.Create<string>();
            var depreciationUri = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""deprecated"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, depreciationUri);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.DepreciationUri == depreciationUri)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingName()
        {
            var href = Fixture.Create<string>();
            var name = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""name"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, name);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Name == name)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingHreflangAsLanguageTag()
        {
            var href = Fixture.Create<string>();
            var hreflang = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""hreflang"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, hreflang);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.LanguageTag == hreflang)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingProfileUri()
        {
            var href = Fixture.Create<string>();
            var profileUri = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""profile"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, profileUri);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.ProfileUri == profileUri)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionUriIsTemplatedForTrue()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""templated"" : true }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.UriIsTemplated)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionUriIsTemplatedForFalse()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""templated"" : false }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.UriIsTemplated == false)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingTitleAndType()
        {
            var href = Fixture.Create<string>();
            var title = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var type = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""title"" : ""{2}"", ""type"" : ""{3}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, title, type);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Title == title && t.Type == type)));
        }

        [Test]
        public void DeserializeToNewBuilder_DoesNotThrowForEmptyDocument()
        {
            const string json = @"{ }";

            Assert.DoesNotThrow(() => sut.DeserializeToNewBuilder(json, builderFactoryMethod));
        }

        [Test]
        public void DeserializeToNewBuilder_AddsSingleEmbeddedResource()
        {
            var href = Fixture.Create<string>();
            var id = Fixture.Create<string>();
            var intId = Fixture.Create<int>();
            var key = Fixture.Create<string>();
            var json = @"
            {{
                ""_embedded"" : {{ ""{0}"" : {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{1}""
                                }}
                }},
                ""id"" : ""{2}"",
                ""int_id"" : ""{3}""
                }} }}
            }}";

            json = String.Format(json, key, href, id, intId);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddEmbeddedResource(Arg<string>.Is.Equal(key), Arg<CrichtonRepresentor>.Is.Anything), options => options.Repeat.Once());

        }

        [Test]
        public void DeserializeToNewBuilder_AddsMultipleEmbeddedResources()
        {
            var href = Fixture.Create<string>();
            var href2 = Fixture.Create<string>();
            var key = Fixture.Create<string>();
            var json = @"
            {{
                ""_embedded"" : {{ ""{0}"" : [ 
                {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{1}""
                                }} }}
                }},
                {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{2}""
                                }} }}
                }} 
                ]
                }}
            }}";

            json = String.Format(json, key, href, href2);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddEmbeddedResource(Arg<string>.Is.Equal(key), Arg<CrichtonRepresentor>.Is.Anything), options => options.Repeat.Twice());

        }

        [Test]
        public void DeserializeToNewBuilder_AddsCollectionForEmbeddedItemsKeyWithMultipleItems()
        {
            var href = Fixture.Create<string>();
            var href2 = Fixture.Create<string>();
            var key = "items";
            var json = @"
            {{
                ""_embedded"" : {{ ""{0}"" : [ 
                {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{1}""
                                }} }}
                }},
                {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{2}""
                                }} }}
                }} 
                ]
                }}
            }}";

            json = String.Format(json, key, href, href2);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var calledCollection = (IEnumerable<CrichtonRepresentor>)builder.GetArgumentsForCallsMadeOn(b => b.SetCollection(null)).First()[0];
            
            Assert.AreEqual(2, calledCollection.Count());
           
        }

        [Test]
        public void DeserializeToNewBuilder_AddsCollectionForEmbeddedItemsKeyWithSingleItem()
        {
            var href = Fixture.Create<string>();
            const string key = "items";
            var json = @"
            {{
                ""_embedded"" : {{ ""{0}"" : 
                {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""{1}""
                                }} }}
                
                }}
                }}
            }}";

            json = String.Format(json, key, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            var calledCollection = (IEnumerable<CrichtonRepresentor>)builder.GetArgumentsForCallsMadeOn(b => b.SetCollection(null)).First()[0];

            Assert.AreEqual(1, calledCollection.Count());

        }

    }
}
