using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    /// <summary>
    /// CrichtonRepresentor class
    /// </summary>
    public class CrichtonRepresentor
    {
        /// <summary>
        /// Gets or sets the SelfLink
        /// </summary>
        public string SelfLink { get; set; }

        /// <summary>
        /// Gets or sets the Attributes
        /// </summary>
        public JObject Attributes { get; set; }

        /// <summary>
        /// Gets or sets the Transitions
        /// </summary>
        public IList<CrichtonTransition> Transitions { get; set; }

        /// <summary>
        /// Gets the EmbeddedResources
        /// </summary>
        public Dictionary<string, IList<CrichtonRepresentor>> EmbeddedResources { get; private set; }

        /// <summary>
        /// Gets the Collectiona
        /// </summary>
        public ICollection<CrichtonRepresentor> Collection { get; private set; } 

        /// <summary>
        /// Initializes a new instance of the CrichtonRepresentor class.
        /// </summary>
        public CrichtonRepresentor()
        {
            Transitions = new List<CrichtonTransition>();
            EmbeddedResources = new Dictionary<string, IList<CrichtonRepresentor>>();
            Collection = new List<CrichtonRepresentor>();
        }

        /// <summary>
        /// Converts to object
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <returns>converted object</returns>
        public T ToObject<T>()
        {
            return Attributes.ToObject<T>();
        }

        /// <summary>
        /// Sets attributes from object
        /// </summary>
        /// <param name="data">the data</param>
        public void SetAttributesFromObject(object data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            Attributes = JObject.FromObject(data);
        }
    }
}
