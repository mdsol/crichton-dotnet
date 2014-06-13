using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public interface IRepresentorBuilder
    {
        /// <summary>
        /// Create a CrichtonRepresentor object from this builder instance.
        /// </summary>
        /// <returns>A CrichtonRepresentor object.</returns>
        CrichtonRepresentor ToRepresentor();

        /// <summary>
        /// Sets the Self Link on the CrichtonRepresentor object that you are building
        /// </summary>
        /// <param name="self">The Self Link Uri</param>
        void SetSelfLink(string self);

        /// <summary>
        /// Sets the CrichtonRepresentor attributes from a JSON.NET JObject.
        /// </summary>
        /// <param name="attributes">A JSON.NET JObject</param>
        void SetAttributes(JObject attributes);

        /// <summary>
        /// Sets the CrichtonRepresentor attributes from an abitrary object. This object will be internally converted into a JObject using JSON.NET, so all standard JSON.NET attributes apply.
        /// </summary>
        /// <param name="data">An object, Model, ViewModel etc</param>
        void SetAttributesFromObject(object data);

        /// <summary>
        /// Adds a CrichtonTransition to the current CrichtonRepresentor that you are building.
        /// </summary>
        /// <param name="transition">A CrichtonTransition object</param>
        void AddTransition(CrichtonTransition transition);

        /// <summary>
        /// Adds a transition to the current CrichtonRepresentor that you are building.
        /// </summary>
        /// <param name="rel">The link relation.</param>
        /// <param name="uri">The Uri of the transition</param>
        /// <param name="title">The title of the transition</param>
        /// <param name="type">The type of the transition</param>
        /// <param name="uriIsTemplated">True if the Uri is a templated Uri, false if not.</param>
        /// <param name="depreciationUri">If the transition has been deprecated, a link to a Uri explaining the deprecation</param>
        /// <param name="name">The name of the transition. Can be used as an alternative or subcategory of title.</param>
        /// <param name="profileUri">Uri to an http://alps.io/ or similar profile.</param>
        /// <param name="languageTag">Language of the transition, as per RFC 5988 http://tools.ietf.org/html/rfc5988</param>
        void AddTransition(string rel, string uri = null, string title = null, string type = null,
            bool uriIsTemplated = false, string depreciationUri = null, string name = null, string profileUri = null, string languageTag = null);

        /// <summary>
        /// Adds an embedded resource. There can be multiple resources with the same key, in which case a collection of resources will be built.
        /// </summary>
        /// <param name="key">The embedded resource key</param>
        /// <param name="resource">The embedded resource as represented by a CrichtonRepresentor</param>
        void AddEmbeddedResource(string key, CrichtonRepresentor resource);

        /// <summary>
        /// Sets the CrichtonRepresentor you are building to be a collection instead of a single object.
        /// </summary>
        /// <param name="representors">An enumerable of CrichtonRepresentors</param>
        void SetCollection(IEnumerable<CrichtonRepresentor> representors);

        /// <summary>
        /// Sets the CrichtonRepresentor you are building to be a collection instead of a single object.
        /// </summary>
        /// <typeparam name="T">The type of the object contained in the collection, such as a Model or ViewModel type</typeparam>
        /// <param name="collection">The collection of objects that represent the collection. JSON.NET will be used to serialize the objects, so the objects can have standard JSON.NET attributes.</param>
        /// <param name="selfLinkFunc">A function that defines the Self Link. This will be called on each object to populate the Self Link of the CrichtonRepresentor.</param>
        void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc);
    }
}