using System;
using Crichton.Representors.Serializers;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Representors.Tests.Serializers
{
    public class JsonSerializerTests : TestWithFixture
    {
        private JsonSerializer sut;
        private Func<IRepresentorBuilder> builderFactoryMethod;
            
        [SetUp]
        public void Init()
        {
            sut = new JsonSerializer();
            Fixture = GetFixture();
            builderFactoryMethod = () => MockRepository.GenerateMock<IRepresentorBuilder>();
        }

        [Test]
        public void HasCorrectContentType()
        {
            Assert.AreEqual("application/json", sut.ContentType);
        }

        [Test]
        public void Serialize_WorksWithEmptyRepresentor()
        {
            var representor = new CrichtonRepresentor();

            sut.Serialize(representor);
        }

        [Test]
        public void Serialize_EmptyAttributes()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            representor.Attributes = new JObject();
            var result = sut.Serialize(representor);
        
            Assert.AreEqual("{}", result);
        }

        [Test]
        public void Serialize_AddsAttributesToRootForEachAttributeInRepresentor()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var dataJobject = JObject.FromObject(Fixture.Create<ExampleDataObject>());

            representor.Attributes = dataJobject;

            var result = JObject.Parse(sut.Serialize(representor));

            foreach (var property in dataJobject.Properties())
            {
                Assert.AreEqual(dataJobject.GetValue(property.Name), result.GetValue(property.Name));
            }
        }

        [Test]
        public void Serialize_AddsNestedAttributeToRootInRepresentor()
        {
            var representor = Fixture.Create<CrichtonRepresentor>();
            var json = @"
            {
                ""data"": {
                    ""self"": {
                        ""not-href"": ""blah""
                                }
                }
            }";

            var dataJobject = JObject.Parse(json);
            representor.Attributes = dataJobject;

            var result = sut.Serialize(representor);

            Assert.AreEqual(dataJobject.ToString(), result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_SetsRepresentorWithNull()
        {
            sut.Serialize(null);
        }

        [Test]
        public void DeserializeToNewBuilder_SetsEmptyDocument()
        {
            var href = Fixture.Create<string>();
            var json = "{}";

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.SetAttributes(Arg<JObject>.Matches(j => j.Count == 0)));

        }

        [Test]
        public void DeserializeToNewBuilder_SetsAttributes()
        {
            var id = Fixture.Create<string>();
            var intId = Fixture.Create<int>();
            var json = @"
            {{
                ""id"" : ""{0}"",
                ""int_id"" : {1}
            }}";

            json = String.Format(json, id, intId);

            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.SetAttributes(Arg<JObject>.Matches(j => j["id"].Value<string>() == id && j["int_id"].Value<int>() == intId)));

        }

        [Test]
        public void DeserializeToNewBuilder_SetsNestedAttribute()
        {
            var href = Fixture.Create<string>();
            var json = @"
            {
                ""data"": {
                    ""self"": {
                        ""not-href"": ""blah""
                                }
                }
            }";

            var dataJobject = JObject.Parse(json);
            var builder = sut.DeserializeToNewBuilder(json, builderFactoryMethod);

            builder.AssertWasCalled(b => b.SetAttributes(Arg<JObject>.Matches(j => j.ToString() == dataJobject.ToString())));

        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeToNewBuilder_SetsMessageWithNull()
        {
            sut.DeserializeToNewBuilder(null, builderFactoryMethod);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeToNewBuilder_SetsMethodWithNull()
        {
            var href = Fixture.Create<string>();
            var json = @"
            {{
                ""data"": {{
                    ""self"": {{
                        ""href"": ""{0}""
                                }}
                }}
            }}";

            var builder = sut.DeserializeToNewBuilder(json, null);
        }
    }
}
