using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crichton.Client.QuerySteps;
using Crichton.Representors;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Crichton.Client.Tests.QuerySteps
{
    public class NavigateToRepresentorQueryStepTests : TestWithFixture
    {
        private NavigateToRepresentorQueryStep sut;
        private CrichtonRepresentor representor;

        [SetUp]
        public void Init()
        {
            Fixture = GetFixture();
            representor = Fixture.Create<CrichtonRepresentor>();
            sut = new NavigateToRepresentorQueryStep(representor);
        }

        [Test]
        public async Task ExecuteAsync_ReturnsConstructorSetRepresentor()
        {
            var result = await sut.ExecuteAsync(null, null);

            Assert.AreEqual(result, representor);
        }
    }
}
