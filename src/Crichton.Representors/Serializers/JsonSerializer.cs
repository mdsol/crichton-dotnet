using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    /// <summary>
    /// JsonSerializer class
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        /// <summary>Gets the IgnoredAttributes</summary>
        public virtual IEnumerable<string> IgnoredAttributes { get { return Enumerable.Empty<string>(); } }

        /// <summary>Gets the ContentType</summary>
        public virtual string ContentType { get { return "application/json"; } }

        /// <summary>
        /// Serializes the representor
        /// </summary>
        /// <param name="representor">the representor</param>
        /// <returns>serialized string</returns>
        public virtual string Serialize(CrichtonRepresentor representor)
        {
            if (representor == null)
            {
                throw new ArgumentNullException("representor");
            }

            return SerializeAttributesToJObject(representor).ToString();
        }

        /// <summary>
        /// Deserializes to new builder
        /// </summary>
        /// <param name="message">the message</param>
        /// <param name="builderFactoryMethod">the builderFactoryMethod</param>
        /// <returns>a builder</returns>
        public virtual IRepresentorBuilder DeserializeToNewBuilder(string message, Func<IRepresentorBuilder> builderFactoryMethod)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (builderFactoryMethod == null)
            {
                throw new ArgumentNullException("builderFactoryMethod");
            }

            var document = JObject.Parse(message);

            var builder = builderFactoryMethod();

            SetAttributes(document, builder);

            return builder;
        }

        protected JObject SerializeAttributesToJObject(CrichtonRepresentor representor)
        {
            var jObject = new JObject();

            if (representor.Attributes != null)
            {
                foreach (var property in representor.Attributes.Properties().Where(p => !IgnoredAttributes.Contains(p.Name)))
                {
                    jObject.Add(property.Name, property.Value);
                }
            }

            return jObject;
        }

        protected void SetAttributes(JObject document, IRepresentorBuilder builder)
        {
            builder.SetAttributes(document);
        }
    }
}