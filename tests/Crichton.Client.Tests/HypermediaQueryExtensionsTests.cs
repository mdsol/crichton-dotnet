using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Rhino.Mocks;

namespace Crichton.Client.Tests
{
    public class HypermediaQueryExtensionsTests : TestWithFixture
    {
        private IHypermediaQuery query;

        [SetUp]
        public void Init()
        {
            query = MockRepository.GenerateMock<IHypermediaQuery>();
            Fixture = GetFixture();
        }

        [Test]
        public void FollowSelf_ReturnsClonedVersionOfHypermediaQuery()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            var result = query.FollowSelf();

            Assert.AreSame(cloneQuery, result);
        }

        [Test]
        public void FollowSelf_CallsAddStepWithNavigateToSelfLinkQueryStep()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            query.FollowSelf();

            cloneQuery.AssertWasCalled(q => q.AddStep(Arg<NavigateToSelfLinkQueryStep>.Is.TypeOf));
        }

        [Test]
        public void Follow_ReturnsClonedVersionOfHypermediaQuery()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var rel = Fixture.Create<string>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            var result = query.Follow(rel);

            Assert.AreSame(cloneQuery, result);
        }

        [Test]
        public void Follow_CallsNavigateToTransitionQueryStepWithRel()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var rel = Fixture.Create<string>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            query.Follow(rel);

            var testRepresentor = Fixture.Create<CrichtonRepresentor>();
            var testTransition = Fixture.Create<CrichtonTransition>();
            testTransition.Rel = rel;
            testRepresentor.Transitions.Add(testTransition);

            cloneQuery.AssertWasCalled(q => q.AddStep(Arg<NavigateToTransitionQueryStep>.Matches(a => a.LocateTransition(testRepresentor) == testTransition)));
        }

        [Test]
        public void FollowWithData_ReturnsClonedVersionOfHypermediaQuery()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var rel = Fixture.Create<string>();
            var data = new { id = 1, name = "Chad" };
            query.Stub(q => q.Clone()).Return(cloneQuery);

            var result = query.FollowWithData(rel, data);

            Assert.AreSame(cloneQuery, result);
        }

        [Test]
        public void FollowWithData_CallsNavigateToTransitionQueryStepWithRelAndData()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var rel = Fixture.Create<string>();
            var data = new { id = 2, name = "Jordi" };
            query.Stub(q => q.Clone()).Return(cloneQuery);

            query.FollowWithData(rel, data);

            var testRepresentor = Fixture.Create<CrichtonRepresentor>();
            var testTransition = Fixture.Create<CrichtonTransition>();
            testTransition.Rel = rel;
            testRepresentor.Transitions.Add(testTransition);

            cloneQuery.AssertWasCalled(q => q.AddStep(Arg<PostToTransitionQueryStep>.Matches(a => a.LocateTransition(testRepresentor) == testTransition && a.Data == data)));
        }

        [Test]
        public void WithUrl_ReturnsClonedVersionOfHypermediaQuery()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var url = Fixture.Create<string>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            var result = query.WithUrl(url);

            Assert.AreSame(cloneQuery, result);
        }

        [Test]
        public void WithUrl_AddsNavigateToRelativeUrlQueryStepWithUrl()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var url = Fixture.Create<string>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            query.WithUrl(url);

            cloneQuery.AssertWasCalled(q => q.AddStep(Arg<NavigateToRelativeUrlQueryStep>.Matches(a => a.Url == url)));
        }

        [Test]
        public void WithRepresentor_ReturnsClonedVersionOfHypermediaQuery()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var representor = Fixture.Create<CrichtonRepresentor>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            var result = query.WithRepresentor(representor);

            Assert.AreSame(cloneQuery, result);
        }


        [Test]
        public void WithRepresentor_AddsNavigateToRepresentorQueryStepWithRepresentor()
        {
            var cloneQuery = Fixture.Create<IHypermediaQuery>();
            var representor = Fixture.Create<CrichtonRepresentor>();
            query.Stub(q => q.Clone()).Return(cloneQuery);

            query.WithRepresentor(representor);

            cloneQuery.AssertWasCalled(
                q => q.AddStep(Arg<NavigateToRepresentorQueryStep>.Matches(r => r.Representor == representor)));
        }
    }
}
