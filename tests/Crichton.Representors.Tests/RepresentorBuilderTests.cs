using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Crichton.Representors.Tests
{
    public class RepresentorBuilderTests : TestWithFixture
    {
        private RepresentorBuilder sut;

        [SetUp]
        public void Init()
        {
            sut = new RepresentorBuilder();
            Fixture = GetFixture();
        }

        [Test]
        public void ToRepresentor_ReturnsARepresentor()
        {
            var result = sut.ToRepresentor();

            Assert.IsInstanceOf<CrichtonRepresentor>(result);
        }

        [Test]
        public void SetSelfLink_SetsSelfLink()
        {
            var self = Fixture.Create<string>();

            sut.SetSelfLink(self);
            var result = sut.ToRepresentor();

            Assert.AreEqual(self, result.SelfLink);
        }

        [Test]
        public void SetAttributes_SetsRepresentorAttributes()
        {
            var attributes = new JObject();
            sut.SetAttributes(attributes);
            var result = sut.ToRepresentor();

            Assert.AreEqual(attributes, result.Attributes);
        }

        [Test]
        public void SetAttributesFromObject_SetsAttributes()
        {
            var example = Fixture.Create<ExampleDataObject>();
            var expectedJObject = JObject.FromObject(example);

            sut.SetAttributesFromObject(example);
            var result = sut.ToRepresentor();

            foreach (var property in expectedJObject.Properties())
            {
                Assert.AreEqual(expectedJObject[property.Name], result.Attributes[property.Name]);
            }

        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransition()
        {
            var rel = Fixture.Create<string>();
            var uri = Fixture.Create<string>();

            sut.AddTransition(rel, uri);
            var result = sut.ToRepresentor();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Uri == uri);

        }

        [Test]
        public void AddTransition_CorrectlyAddsSimpleTransitionWithTitle()
        {
            var rel = Fixture.Create<string>();
            var uri = Fixture.Create<string>();
            var title = Fixture.Create<string>();

            sut.AddTransition(rel, uri, title);
            var result = sut.ToRepresentor();

            result.Transitions.Should().ContainSingle(t => t.Rel == rel && t.Uri == uri && t.Title == title);

        }

        [Test]
        public void AddEmbeddedResource_AddsResourceWithCorrectKey()
        {
            var key = Fixture.Create<string>();
            var resource = Fixture.Create<CrichtonRepresentor>();

            sut.AddEmbeddedResource(key, resource);
            var result = sut.ToRepresentor();

            result.EmbeddedResources[key].Should().ContainSingle(t => t == resource);
        }
    }
}
