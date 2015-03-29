using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HalKit.Models;
using HalKit.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HalKit.Json
{
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

            resource.Links = DeserializeLinks(json);

            DeserializeAndAssignEmbeddedProperties(json, objectType, ref resource);

            return resource;
        }

        LinkCollection DeserializeLinks(JToken json)
        {
            var links = new List<Link>();
            var curies = new List<CurieLink>();
            if (json["_links"] == null || !json["_links"].HasValues)
            {
                return new LinkCollection(links, curies);
            }

            var enumerator = ((JObject)json["_links"]).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var rel = enumerator.Current.Key;
                var token = enumerator.Current.Value;
                if (rel == "curies")
                {
                    curies.AddRange(token.Select(c => c.ToObject<CurieLink>()));
                    continue;
                }

                // Links can be one object or an array of objects
                var linksForThisRel = token.Type == JTokenType.Array
                                        ? token.Select(t => t)
                                        : new[] {token};
                foreach (var linkToken in linksForThisRel)
                {
                    var link = linkToken.ToObject<Link>();
                    link.Rel = rel;
                    links.Add(link);
                }
            }

            return new LinkCollection(links, curies);
        }

        void DeserializeAndAssignEmbeddedProperties(JToken json, Type objectType, ref Resource resource)
        {
            if (json["_embedded"] == null || !json["_embedded"].HasValues)
            {
                return;
            }

            var embeddedPropertiesMap = new Dictionary<string, PropertyInfo>();
            foreach (var property in objectType.GetTypeInfo().DeclaredProperties)
            {
                var embeddedAttribute = property.GetCustomAttribute<EmbeddedAttribute>(true);
                if (embeddedAttribute == null)
                {
                    // This property doesn't have the EmbeddedAttribute
                    continue;
                }

                embeddedPropertiesMap.Add(embeddedAttribute.Rel, property);
            }

            if (embeddedPropertiesMap.Count == 0)
            {
                // No properties in this object are mapped to anything
                // in _embedded so bail
                return;
            }

            var enumerator = ((JObject)json["_embedded"]).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var rel = enumerator.Current.Key;
                PropertyInfo property;
                if (!embeddedPropertiesMap.TryGetValue(rel, out property))
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
            get { return false; }
        }
    }
}
