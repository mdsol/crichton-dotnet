using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Crichton.Representors.Tests
{
    public class CrichtonRepresentorTests : TestWithFixture
    {
        private CrichtonRepresentor sut;

        [SetUp]
        public void Init()
        {
            sut = new CrichtonRepresentor();
            Fixture = GetFixture();
        }

        [Test]
        public void ToObject_CorrectlyDeserializesAttributes()
        {
            var expected = Fixture.Create<ExampleDataObject>();
            sut.Attributes = JObject.FromObject(expected);

            var result = sut.ToObject<ExampleDataObject>();

            result.ShouldBeEquivalentTo(expected, options => options.IncludingAllDeclaredProperties());
        }

        [Test]
        public void SetAttributesFromObject_SetsAttributesMatchingJObjectRepresentationOfObject()
        {
            var expected = Fixture.Create<ExampleDataObject>();
            var expectedJObject = JObject.FromObject(expected);

            sut.SetAttributesFromObject(expected);

            foreach (var property in expectedJObject.Properties())
            {
                Assert.AreEqual(expectedJObject[property.Name], sut.Attributes[property.Name]);
            }

        }

        [Test]
        public void SetAttributes_RoundTrip()
        {
            var expected = Fixture.Create<ExampleDataObject>();
            
            sut.SetAttributesFromObject(expected);

            var result = sut.ToObject<ExampleDataObject>();

            result.ShouldBeEquivalentTo(expected, options => options.IncludingAllDeclaredProperties());
        }
    }
}
