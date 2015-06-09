using System;
using System.Collections.Generic;
using System.Reflection;
using HalKit.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HalKit.Json
{
    /// <summary>
    /// Converts a <see cref="Resource"/> to an from JSON.
    /// </summary>
    public class ResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var json = JObject.ReadFrom(reader);
            var resource = (Resource)JsonConvert.DeserializeObject(json.ToString(), objectType);

            var jsonHasEmbedded = json["_embedded"] != null && json["_embedded"].HasValues;
            var jsonHasLinks = json["_links"] != null && json["_links"].HasValues;
            if (!jsonHasEmbedded && !jsonHasLinks)
            {
                // No _embedded or _links in the JSON so return the resource as is
                return resource;
            }

            var embeddedPropertiesMap = new Dictionary<string, PropertyInfo>();
            var linkPropertiesMap = new Dictionary<string, PropertyInfo>();
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

            DeserializeAndAssignProperties(json, "_links", linkPropertiesMap, ref resource);
            DeserializeAndAssignProperties(json, "_embedded", embeddedPropertiesMap, ref resource);

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
    }
}
