using System;
using System.Collections.Generic;
using System.Reflection;
using HalKit.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace HalKit.Json
{
    /// <summary>
    /// Resolves a <see cref="JsonContract"/> for a given <see cref="Resource"/>.
    /// </summary>
    public class ResourceContractResolver : DefaultContractResolver
    {
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceContractResolver"/>
        /// class.
        /// </summary>
        public ResourceContractResolver(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var allProperties = base.CreateProperties(type, memberSerialization);
            if (!typeof(Resource).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return allProperties;
            }

            var contractProperties = new List<JsonProperty>();
            var embeddedPropertyMap = new Dictionary<string, JsonProperty>();
            var linksPropertyMap = new Dictionary<string, JsonProperty>();
            foreach (var property in allProperties)
            {
                var isLinkOrEmbeddedProperty = false;
                var attributes = property.AttributeProvider.GetAttributes(false);
                foreach (var attribute in attributes)
                {
                    var embeddedAttribute = attribute as EmbeddedAttribute;
                    if (embeddedAttribute != null)
                    {
                        isLinkOrEmbeddedProperty = true;
                        embeddedPropertyMap.Add(embeddedAttribute.Rel, property);
                    }

                    var relAttribute = attribute as RelAttribute;
                    if (relAttribute != null)
                    {
                        isLinkOrEmbeddedProperty = true;
                        linksPropertyMap.Add(relAttribute.Rel, property);
                    }
                }

                // This doesn't have a Rel or Embedded attribute so it's just a normal property
                if (!isLinkOrEmbeddedProperty)
                {
                    contractProperties.Add(property);
                }
            }

            contractProperties.Add(CreateReservedHalJsonProperty(type, "_links", linksPropertyMap));
            contractProperties.Add(CreateReservedHalJsonProperty(type, "_embedded", embeddedPropertyMap));

            return contractProperties;
        }

        private JsonProperty CreateReservedHalJsonProperty(
            Type type,
            string name,
            IReadOnlyDictionary<string, JsonProperty> propertyMap)
        {
            return new JsonProperty
            {
                PropertyName = name,
                PropertyType = typeof(Dictionary<string, object>),
                DeclaringType = type,
                ValueProvider = new ReservedHalPropertyValueProvider(_settings, propertyMap),
                NullValueHandling = NullValueHandling.Ignore,
                Readable = propertyMap.Values.Any(p => p.Readable),
                Writable = propertyMap.Values.Any(p => p.Writable),
                ShouldSerialize = o => true,
                GetIsSpecified = o => true,
                SetIsSpecified = null,
                Order = int.MaxValue,
            };
        }

        private class ReservedHalPropertyValueProvider : IValueProvider
        {
            private readonly JsonSerializerSettings _settings;
            private readonly IReadOnlyDictionary<string, JsonProperty> _propertyMap;

            public ReservedHalPropertyValueProvider(
                JsonSerializerSettings settings,
                IReadOnlyDictionary<string, JsonProperty> propertyMap)
            {
                _settings = settings;
                _propertyMap = propertyMap;
            }

            public object GetValue(object target)
            {
                // Use a SortedDictionary since it just seems "right" for the
                // "self" link to be first
                var reservedPropertyValue = new SortedDictionary<string, object>(new RelComparer());
                foreach (var rel in _propertyMap.Keys)
                {
                    var property = _propertyMap[rel];
                    var propertyValue = property.ValueProvider.GetValue(target);
                    if (propertyValue != null)
                    {
                        reservedPropertyValue.Add(rel, propertyValue);
                    }
                }

                return reservedPropertyValue.Count > 0 ? reservedPropertyValue : null;
            }

            public void SetValue(object target, object value)
            {
                var valueDictionary = value as IDictionary<string, object>;
                if (valueDictionary == null)
                {
                    return;
                }

                foreach (var rel in valueDictionary.Keys)
                {
                    JsonProperty property;
                    if (!_propertyMap.TryGetValue(rel, out property))
                    {
                        continue;
                    }

                    var serializer = JsonSerializer.Create(_settings);
                    var propertyJson = valueDictionary[rel] as JToken;
                    var propertyValue = propertyJson != null
                                            ? propertyJson.ToObject(property.PropertyType, serializer)
                                            : null;
                    property.ValueProvider.SetValue(target, propertyValue);
                }
            }
        }

        private class RelComparer : IComparer<string>
        {
            public int Compare(string rel, string otherRel)
            {
                const string self = "self";
                if (rel == self)
                {
                    return -1;
                }

                if (otherRel == self)
                {
                    return 1;
                }

                return string.Compare(rel, otherRel, StringComparison.Ordinal);
            }
        }
    }
}
