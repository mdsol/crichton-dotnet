using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;

namespace Crichton.Representors.Tests
{
    public class TestWithFixture
    {
        internal IFixture Fixture;

        public IFixture GetFixture()
        {
            return new Fixture().Customize(new MultipleCustomization()).Customize(new AutoRhinoMockCustomization());
        }
    }
}
