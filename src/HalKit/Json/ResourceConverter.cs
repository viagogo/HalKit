using System;
using System.Collections.Generic;
using System.Reflection;
using HalKit.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace HalKit.Json
{
    /// <summary>
    /// Converts a <see cref="Resource"/> to an from JSON.
    /// </summary>
    public class ResourceConverter : JsonConverter
    {
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

            Dictionary<string, PropertyInfo> linkPropertiesMap;
            Dictionary<string, PropertyInfo> embeddedPropertiesMap;
            GetPropertiesWithRelAndEmbeddedAttributes(
                resource.GetType(),
                out linkPropertiesMap,
                out embeddedPropertiesMap);

            AddReservedHalProperty(Links, linkPropertiesMap, resource, resourceJson, serializer);
            AddReservedHalProperty(Embedded, embeddedPropertiesMap, resource, resourceJson, serializer);

            resourceJson.WriteTo(writer, serializer.Converters.ToArray());
        }

        private void AddReservedHalProperty(
            string reservedPropertyName,
            IDictionary<string, PropertyInfo> relToPropertyMap,
            object resource,
            JObject resourceJson,
            JsonSerializer serializer)
        {
            var reservedPropertyJson = new JObject();
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

                reservedPropertyJson.Add(rel, JToken.FromObject(propertyValue, serializer));
            }

            if (reservedPropertyJson.Count > 0)
            {
                resourceJson.Add(reservedPropertyName, reservedPropertyJson);
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

            Dictionary<string, PropertyInfo> linkPropertiesMap;
            Dictionary<string, PropertyInfo> embeddedPropertiesMap;
            GetPropertiesWithRelAndEmbeddedAttributes(
                objectType,
                out linkPropertiesMap,
                out embeddedPropertiesMap);

            DeserializeAndAssignProperties(json, Links, linkPropertiesMap, ref resource);
            DeserializeAndAssignProperties(json, Embedded, embeddedPropertiesMap, ref resource);

            return resource;
        }

        private void DeserializeAndAssignProperties(
            JToken json,
            string jsonKey,
            IDictionary<string, PropertyInfo> propertiesMap,
            ref Resource resource)
        {
            if (propertiesMap == null)
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

        private void GetPropertiesWithRelAndEmbeddedAttributes(
            Type objectType,
            out Dictionary<string, PropertyInfo> linkPropertiesMap,
            out Dictionary<string, PropertyInfo> embeddedPropertiesMap)
        {
            // TODO: We could probably have a static cache of these lookups for
            // each type
            linkPropertiesMap = new Dictionary<string, PropertyInfo>();
            embeddedPropertiesMap = new Dictionary<string, PropertyInfo>();
            foreach (var property in objectType.GetTypeInfo().DeclaredProperties)
            {
                var embeddedAttribute = property.GetCustomAttribute<EmbeddedAttribute>(true);
                if (embeddedAttribute != null)
                {
                    embeddedPropertiesMap.Add(embeddedAttribute.Rel, property);
                }

                var linkAttribute = property.GetCustomAttribute<RelAttribute>(true);
                if (linkAttribute != null)
                {
                    linkPropertiesMap.Add(linkAttribute.Rel, property);
                }
            }
        }
    }
}
