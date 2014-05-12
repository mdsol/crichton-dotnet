using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crichton.Representors.Serializers
{
    public class HaleSerializer : HalSerializer
    {
        private static readonly Dictionary<TransitionRenderMethod, string>
            RenderMethodMappings = new Dictionary<TransitionRenderMethod, string> { { TransitionRenderMethod.Resource, "resource" }, { TransitionRenderMethod.Embed, "embed" } };

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

            foreach (var map in RenderMethodMappings.Where(map => transition.RenderMethod == map.Key))
            {
                linkObject["render"] = map.Value;
            }

            if (!String.IsNullOrWhiteSpace(transition.Target)) linkObject["target"] = transition.Target;

            if (transition.Attributes != null && transition.Attributes.Any())
            {
                var dataObject = linkObject["data"] = new JObject();

                foreach (var attribute in transition.Attributes)
                {
                    var attributeObject = new JObject();

                    if (!String.IsNullOrWhiteSpace(attribute.Value.JsonType))
                    {
                        var typeValue = new StringBuilder(attribute.Value.JsonType);
                        if (!String.IsNullOrWhiteSpace(attribute.Value.DataType))
                        {
                            typeValue.Append(":");
                            typeValue.Append(attribute.Value.DataType);
                        }
                        attributeObject["type"] = typeValue.ToString();
                    }

                    if (!String.IsNullOrWhiteSpace(attribute.Value.ProfileUri))
                    {
                        attributeObject["profile"] = attribute.Value.ProfileUri;
                    }

                    if (attribute.Value.Value != null)
                    {
                        attributeObject["value"] = JToken.FromObject(attribute.Value.Value);
                    }

                    dataObject[attribute.Key] = attributeObject;
                }
            }

            return linkObject;
        }

        public override CrichtonTransition GetTransitionFromLinkObject(JToken link, string rel)
        {
            var transition = base.GetTransitionFromLinkObject(link, rel);

            var methods = link["method"];
            var enctype = link["enctype"];
            var render = link["render"];
            var target = link["target"];
            var data = link["data"];

            if (methods != null)
            {
                transition.Methods = methods is JArray ?
                    methods.Values<string>().ToArray() : new[] { methods.Value<string>() };
            }

            if (enctype != null)
            {
                transition.MediaTypesAccepted = enctype is JArray ?
                    enctype.Values<string>().ToArray() : new[] { enctype.Value<string>() };
            }

            if (render != null)
            {
                var renderValue = render.Value<string>();
                if (RenderMethodMappings.ContainsValue(renderValue))
                    transition.RenderMethod = RenderMethodMappings.Single(r => r.Value == renderValue).Key;
            }

            if (target != null) transition.Target = target.Value<string>();

            if (data != null)
            {
                transition.Attributes = new Dictionary<string, CrichtonTransitionAttribute>();

                foreach (var dataProperty in ((JObject) data).Properties())
                {
                    var dataObject = data[dataProperty.Name];
                    var type = dataObject["type"];
                    var profileUri = dataObject["profile"];
                    var value = dataObject["value"];

                    var transitionAttribute = new CrichtonTransitionAttribute();

                    if (type != null)
                    {
                        var splitType = type.Value<string>().Split(':');
                        transitionAttribute.JsonType = splitType[0];
                        if (splitType.Count() > 1) transitionAttribute.DataType = splitType[1];
                    }

                    if (profileUri != null) transitionAttribute.ProfileUri = profileUri.Value<string>();

                    if (value != null) transitionAttribute.Value = JsonConvert.DeserializeObject(value.ToString());

                    transition.Attributes[dataProperty.Name] = transitionAttribute;
                }
            }

            return transition;
        }
    }
}
