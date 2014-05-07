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
                    var methodArray = new JArray();
                    foreach (var method in transition.Methods)
                    {
                        methodArray.Add(method);
                    }
                    linkObject["method"] = methodArray;
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
                    var methodArray = new JArray();
                    foreach (var method in transition.MediaTypesAccepted)
                    {
                        methodArray.Add(method);
                    }
                    linkObject["enctype"] = methodArray;
                }
            }

            return linkObject;
        }

        public override CrichtonTransition GetTransitionFromLinkObject(JToken link, string rel)
        {
            var transition = base.GetTransitionFromLinkObject(link, rel);

            var methods = link["method"];

            if (methods != null)
            {
                var methodsAsArray = methods as JArray;
                if (methodsAsArray == null)
                {
                    transition.Methods = new[] {methods.Value<string>()};
                }
                else
                {
                    transition.Methods = methods.Values<string>().ToArray();
                }
            }

            return transition;
        }
    }
}
