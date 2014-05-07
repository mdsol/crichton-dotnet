using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Representors.Serializers;
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
    }
}
