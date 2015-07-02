using System;
using System.Collections.Generic;
using System.Reflection;
using HalKit.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json.Utilities;
using HalKit.Utilities;

namespace HalKit.Json
{
    /// <summary>
    /// Converts a <see cref="Resource"/> to an from JSON.
    /// </summary>
    public class ResourceConverter : JsonConverter
    {
        private static ThreadSafeStore<Type, LinkAndEmbeddedPropertiesLookup> PropertiesLookupCache
            = new ThreadSafeStore<Type, LinkAndEmbeddedPropertiesLookup>(t => new LinkAndEmbeddedPropertiesLookup(t));

        private const string Links = "_links";
        private const string Embedded = "_embedded";

        public override bool CanConvert(Type objectType)
        {
            return typeof(Resource).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object resource, JsonSerializer serializer)
        {
            // Serialize the "non-Resource" parts of the object into a JToken so
            // that we can add the "resource" parts.
            serializer.Converters.Remove(this);
            var resourceJson = (JObject)JToken.FromObject(resource, serializer);
            serializer.Converters.Add(this);

            var propertiesLookup = PropertiesLookupCache.Get(resource.GetType());
            AddReservedHalProperty(Links, propertiesLookup.LinkPropertiesMap, resource, resourceJson, serializer);
            AddReservedHalProperty(Embedded, propertiesLookup.EmbeddedPropertiesMap, resource, resourceJson, serializer);

            resourceJson.WriteTo(writer, serializer.Converters.ToArray());
        }

        private void AddReservedHalProperty(
            string reservedPropertyName,
            IReadOnlyDictionary<string, PropertyInfo> relToPropertyMap,
            object resource,
            JObject resourceJson,
            JsonSerializer serializer)
        {
            var reservedHalJson = new JObject();
            foreach (var relAndProperty in relToPropertyMap)
            {
                var rel = relAndProperty.Key;
                var propertyValue = relAndProperty.Value.GetValue(resource);
                if (propertyValue == null)
                {
                    // The _links and _embedded properties don't contain null properties
                    // See https://tools.ietf.org/html/draft-kelly-json-hal-06#section-4.1
                    continue;
                }

                var propertyJson = new JProperty(rel, JToken.FromObject(propertyValue, serializer));
                if (rel == "self")
                {
                    // Just seems "right" for the "self" link to be first
                    reservedHalJson.AddFirst(propertyJson);
                }
                else
                {
                    reservedHalJson.Add(propertyJson);
                }
            }

            if (reservedHalJson.Count > 0)
            {
                resourceJson.Add(reservedPropertyName, reservedHalJson);
            }
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var json = JToken.ReadFrom(reader);
            var resource = (Resource)JsonConvert.DeserializeObject(json.ToString(), objectType);

            var jsonHasEmbedded = json[Embedded] != null && json[Embedded].HasValues;
            var jsonHasLinks = json[Links] != null && json[Links].HasValues;
            if (!jsonHasEmbedded && !jsonHasLinks)
            {
                // No _embedded or _links in the JSON so return the resource as is
                return resource;
            }

            var propertiesLookup = PropertiesLookupCache.Get(resource.GetType());
            DeserializeAndAssignProperties(json, Links, jsonHasLinks, propertiesLookup.LinkPropertiesMap, ref resource);
            DeserializeAndAssignProperties(json, Embedded, jsonHasEmbedded, propertiesLookup.EmbeddedPropertiesMap, ref resource);

            return resource;
        }

        private void DeserializeAndAssignProperties(
            JToken json,
            string jsonKey,
            bool jsonHasKey,
            IReadOnlyDictionary<string, PropertyInfo> propertiesMap,
            ref Resource resource)
        {
            if (propertiesMap == null || !jsonHasKey)
            {
                // No properties in this object are mapped with any attributes so bail
                return;
            }
            
            var enumerator = ((JObject)json[jsonKey]).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var rel = enumerator.Current.Key;
                PropertyInfo property;
                if (!propertiesMap.TryGetValue(rel, out property))
                {
                    continue;
                }

                var deserializedPropertyValue = JsonConvert.DeserializeObject(
                                                    enumerator.Current.Value.ToString(),
                                                    property.PropertyType,
                                                    this);
                property.SetValue(resource, deserializedPropertyValue);
            }
        }
    }
}
