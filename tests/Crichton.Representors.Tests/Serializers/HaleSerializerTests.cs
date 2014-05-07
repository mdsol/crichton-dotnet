using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Serializers
{
    public class HaleSerializerTests : TestWithFixture
    {
        private HaleSerializer sut;
        private Func<IRepresentorBuilder> builderFactoryMethod;

        [SetUp]
        public void Init()
        {
            sut = new HaleSerializer();
            Fixture = GetFixture();
            builderFactoryMethod = () => MockRepository.GenerateMock<IRepresentorBuilder>();
        }

        private CrichtonRepresentor GetRepresentorWithTransitions(Func<CrichtonTransition> transitionFunc)
        {
            var result = Fixture.Create<CrichtonRepresentor>();
            result.Transitions.Clear();
            result.Transitions.AddMany(transitionFunc, new Random(DateTime.Now.Millisecond).Next(100));
            return result;
        }

        [Test]
        public void Serialize_AddsSingleMethodAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), Methods = new []{ Fixture.Create<string>() } });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Methods.Single(), result["_links"][transition.Rel]["method"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsMultipleMethodAttributesForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), Methods = Fixture.Create<string[]>() });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                var array = (JArray)result["_links"][transition.Rel]["method"];
                foreach (var method in transition.Methods)
                {
                    Assert.IsTrue(array.Any(a => a.Value<string>() == method));
                }
            }
        }

        [Test]
        public void Serialize_AddsSingleAcceptedMediaTypeAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), MediaTypesAccepted = new[] { Fixture.Create<string>() } });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.MediaTypesAccepted.Single(), result["_links"][transition.Rel]["enctype"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsMultipleAcceptedMediaTypeAttributesForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), MediaTypesAccepted = Fixture.Create<string[]>() });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                var array = (JArray)result["_links"][transition.Rel]["enctype"];
                foreach (var mediaType in transition.MediaTypesAccepted)
                {
                    Assert.IsTrue(array.Any(a => a.Value<string>() == mediaType));
                }
            }
        }

        [Test]
        public void Serialize_DoesNotAddAttributeForUndefinedRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Undefined });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.IsNull(result["_links"][transition.Rel]["render"]);
            }
        }

        [Test]
        public void Serialize_AddsAttributeForEmbedRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Embed });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual("embed", result["_links"][transition.Rel]["render"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsAttributeForResourceRenderMethod()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), RenderMethod = TransitionRenderMethod.Resource });

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual("resource", result["_links"][transition.Rel]["render"].Value<string>());
            }
        }

        [Test]
        public void Serialize_AddsTargetAttributeForEachTransition()
        {
            var representor = GetRepresentorWithTransitions(() => new CrichtonTransition() { Rel = Fixture.Create<string>(), Target = Fixture.Create<string>() });
            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var transition in representor.Transitions)
            {
                Assert.AreEqual(transition.Target, result["_links"][transition.Rel]["target"].Value<string>());
            }
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingSingleMethod()
        {
            var href = Fixture.Create<string>();
            var method = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""method"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, method);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Methods.Single() == method)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingMultipleMethods()
        {
            var href = Fixture.Create<string>();
            var methods = Fixture.Create<string[]>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""method"" : {2} }} 
                }}
            }}";

            json = String.Format(json, rel, href, JsonConvert.SerializeObject(methods));

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && !t.Methods.Except(methods).Any())));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingSingleMediaType()
        {
            var href = Fixture.Create<string>();
            var mediaType = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""enctype"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, mediaType);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.MediaTypesAccepted.Single() == mediaType)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingMultipleMediaTypes()
        {
            var href = Fixture.Create<string>();
            var mediaTypes = Fixture.Create<string[]>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""enctype"" : {2} }} 
                }}
            }}";

            json = String.Format(json, rel, href, JsonConvert.SerializeObject(mediaTypes));

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && !t.MediaTypesAccepted.Except(mediaTypes).Any())));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsRenderMethodToEmbed()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""embed"" }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Embed)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsRenderMethodToResource()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""resource"" }} 
                }}
            }}";

            json = String.Format(json, rel, href);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Resource)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsToUndefinedForInvalidRenderMethod()
        {
            var href = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var render = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""render"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, render);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.RenderMethod == TransitionRenderMethod.Undefined)));
        }

        [Test]
        public void DeserializeToNewBuilder_SetsTransitionsIncludingTarget()
        {
            var href = Fixture.Create<string>();
            var target = Fixture.Create<string>();
            var rel = Fixture.Create<string>();
            var json = @"
            {{
                ""_links"": {{
                    ""self"": {{
                        ""href"": ""self-url""
                                }},
                    ""{0}"": {{ ""href"" : ""{1}"", ""target"" : ""{2}"" }} 
                }}
            }}";

            json = String.Format(json, rel, href, target);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.AddTransition(Arg<CrichtonTransition>.Matches(t => t.Rel == rel && t.Uri == href && t.Target == target)));
        }
    }
}
