using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HaleSerializer : HalSerializer
    {
        public override JObject CreateLinkObjectFromTransition(CrichtonTransition transition)
        {
            var linkObject = base.CreateLinkObjectFromTransition(transition);

            if (transition.Methods != null && transition.Methods.Any())
            {
                if (transition.Methods.Count() == 1)
                {
                    linkObject["method"] = transition.Methods.Single();
                }
                else
                {
                    linkObject["method"] = new JArray(transition.Methods.ToList());
                }
            }

            if (transition.MediaTypesAccepted != null && transition.MediaTypesAccepted.Any())
            {
                if (transition.MediaTypesAccepted.Count() == 1)
                {
                    linkObject["enctype"] = transition.MediaTypesAccepted.Single();
                }
                else
                {
                    linkObject["enctype"] = new JArray(transition.MediaTypesAccepted.ToList());
                }
            }

            return linkObject;
        }

        public override CrichtonTransition GetTransitionFromLinkObject(JToken link, string rel)
        {
            var transition = base.GetTransitionFromLinkObject(link, rel);

            var methods = link["method"];
            var enctype = link["enctype"];

            if (methods != null)
            {
                transition.Methods = methods is JArray ? 
                    methods.Values<string>().ToArray() : new[] {methods.Value<string>()} ;
            }

            if (enctype != null)
            {
                transition.MediaTypesAccepted = enctype is JArray ? 
                    enctype.Values<string>().ToArray() : new[] { enctype.Value<string>() } ;
            }

            return transition;
        }
    }
}
