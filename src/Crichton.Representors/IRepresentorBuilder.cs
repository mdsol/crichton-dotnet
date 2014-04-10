using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public interface IRepresentorBuilder
    {
        CrichtonRepresentor ToRepresentor();
        void SetSelfLink(string self);
        void SetAttributes(JObject attributes);
        void SetAttributesFromObject(object data);
        void AddTransition(string rel, string uri);
    }
}