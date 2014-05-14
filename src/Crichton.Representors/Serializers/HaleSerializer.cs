using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            AddAttributeesFromAttributesContainerToLinkObject(transition, linkObject);

            return linkObject;
        }

        private static void AddAttributeesFromAttributesContainerToLinkObject(IAttributesContainer attributesContainer, JObject linkObject)
        {
            var onlyInAttributes = attributesContainer.Attributes.Where(
                a => !attributesContainer.Parameters.ContainsKey(a.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var onlyInParameters = attributesContainer.Parameters.Where(
                a => !attributesContainer.Attributes.ContainsKey(a.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var inBoth = attributesContainer.Attributes.Where(a => attributesContainer.Parameters.ContainsKey(a.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            AddAttributesFromAttributesDictionaryToLinkObject(onlyInAttributes, linkObject);
            AddAttributesFromAttributesDictionaryToLinkObject(onlyInParameters, linkObject, "href");
            AddAttributesFromAttributesDictionaryToLinkObject(inBoth, linkObject, "either");
        }

        private static void AddAttributesFromAttributesDictionaryToLinkObject(
            IDictionary<string, CrichtonTransitionAttribute> attributesDictionary,
            JObject linkObject,
            string scope = null)
        {
            if (attributesDictionary == null || !attributesDictionary.Any()) return;

            var dataObject = linkObject["data"] = new JObject();

            foreach (var attribute in attributesDictionary)
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

                if (!String.IsNullOrWhiteSpace(scope))
                {
                    attributeObject["scope"] = scope;
                }

                AddAttributeesFromAttributesContainerToLinkObject(attribute.Value, attributeObject);

                dataObject[attribute.Key] = attributeObject;
            }
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

            SetAttributesAndParametersOnAttributesContainer(data, transition);

            return transition;
        }

        private static void SetAttributesAndParametersOnAttributesContainer(JToken dataToken, IAttributesContainer transition)
        {
            if (dataToken == null) return;

            var noScopeSet = GetAttributesDictionaryFromDataToken(dataToken, null);
            var hrefScopeSet = GetAttributesDictionaryFromDataToken(dataToken, "href");
            var eitherScopeSet = GetAttributesDictionaryFromDataToken(dataToken, "either");
            transition.Attributes = noScopeSet.MergeLeft(eitherScopeSet);
            transition.Parameters = hrefScopeSet.MergeLeft(eitherScopeSet);
        }

        private static Dictionary<string, CrichtonTransitionAttribute> 
            GetAttributesDictionaryFromDataToken(JToken data, string matchingScope)
        {
            var result = new Dictionary<string, CrichtonTransitionAttribute>();

            foreach (var dataProperty in ((JObject) data).Properties())
            {
                var dataObject = data[dataProperty.Name];
                var type = dataObject["type"];
                var profileUri = dataObject["profile"];
                var value = dataObject["value"] as JValue;
                var dataToken = dataObject["data"];
                var scope = dataObject["scope"];

                if (scope == null && matchingScope != null) continue;

                if (scope != null)
                {
                    if (scope.Value<string>() != matchingScope) continue;
                }

                var transitionAttribute = new CrichtonTransitionAttribute();

                if (type != null)
                {
                    var splitType = type.Value<string>().Split(':');
                    transitionAttribute.JsonType = splitType[0];
                    if (splitType.Count() > 1) transitionAttribute.DataType = splitType[1];
                }

                if (profileUri != null) transitionAttribute.ProfileUri = profileUri.Value<string>();

                if (value != null)
                {
                    transitionAttribute.Value = value.Value;
                }

                SetAttributesAndParametersOnAttributesContainer(dataToken, transitionAttribute);

                result[dataProperty.Name] = transitionAttribute;
            }

            return result;
        }
    }
}
