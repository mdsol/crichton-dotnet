using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Crichton.Representors;

namespace Crichton.WebApi
{
    public interface IBuilderDescriptor
    {
        IRepresentorBuilder BuildForType(Type type, object value, HttpRequestContext context);
        bool SupportsType(Type type);
    }
}
