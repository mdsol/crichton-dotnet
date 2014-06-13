using System;

namespace Crichton.Representors.Serializers
{
    public interface ISerializer
    {
        string ContentType { get; }
        string Serialize(CrichtonRepresentor representor);
        IRepresentorBuilder DeserializeToNewBuilder(string message, Func<IRepresentorBuilder> builderFactoryMethod);
    }
}