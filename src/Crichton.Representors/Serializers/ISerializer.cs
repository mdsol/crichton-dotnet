using System;

namespace Crichton.Representors.Serializers
{
    /// <summary>
    /// ISerializer interface
    /// </summary>
    public interface ISerializer
    {
        /// <summary>Gets the IgnoredAttributes</summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the representor
        /// </summary>
        /// <param name="representor">the representor</param>
        /// <returns>serialized string</returns>
        string Serialize(CrichtonRepresentor representor);

        /// <summary>
        /// Deserializes to new builder
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="builderFactoryMethod">the builderFactoryMethod</param>
        /// <returns>a builder</returns>
        IRepresentorBuilder DeserializeToNewBuilder(string message, Func<IRepresentorBuilder> builderFactoryMethod);
    }
}