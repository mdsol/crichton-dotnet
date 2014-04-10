using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors
{
    public class RepresentorBuilder
    {
        private readonly CrichtonRepresentor representor;

        public RepresentorBuilder()
        {
            representor = new CrichtonRepresentor();
        }

        public CrichtonRepresentor ToRepresentor()
        {
            return representor;
        }

        public void SetSelfLink(string self)
        {
            representor.SelfLink = self;
        }

        public void SetAttributes(JObject attributes)
        {
            representor.Attributes = attributes;
        }

        public void SetAttributesFromObject(object data)
        {
            representor.Attributes = JObject.FromObject(data);
        }

        public void AddTransition(string rel, string uri)
        {
            representor.Transitions.Add(new CrichtonTransition() { Rel = rel, Uri = uri});
        }
    }
}
