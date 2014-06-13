using System.Linq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;

namespace Crichton.Representors.Tests
{
    public class TestWithFixture
    {
        internal IFixture Fixture;

        public IFixture GetFixture()
        {
            var fixture = new Fixture().Customize(new MultipleCustomization()).Customize(new AutoRhinoMockCustomization());
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().Single());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}
