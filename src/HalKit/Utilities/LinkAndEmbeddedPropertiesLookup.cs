using HalKit.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HalKit.Utilities
{
    public class LinkAndEmbeddedPropertiesLookup
    {
        private Dictionary<string, PropertyInfo> _linkPropertiesMap;
        private Dictionary<string, PropertyInfo> _embeddedPropertiesMap;

        public LinkAndEmbeddedPropertiesLookup(Type objectType)
        {
            _linkPropertiesMap = new Dictionary<string, PropertyInfo>();
            _embeddedPropertiesMap = new Dictionary<string, PropertyInfo>();
            foreach (var property in objectType.GetRuntimeProperties())
            {
                var embeddedAttribute = property.GetCustomAttribute<EmbeddedAttribute>(true);
                if (embeddedAttribute != null)
                {
                    _embeddedPropertiesMap.Add(embeddedAttribute.Rel, property);
                }

                var linkAttribute = property.GetCustomAttribute<RelAttribute>(true);
                if (linkAttribute != null)
                {
                    _linkPropertiesMap.Add(linkAttribute.Rel, property);
                }
            }
        }

        public IReadOnlyDictionary<string, PropertyInfo> LinkPropertiesMap
        {
            get { return _linkPropertiesMap; }
        }

        public IReadOnlyDictionary<string, PropertyInfo> EmbeddedPropertiesMap
        {
            get { return _embeddedPropertiesMap; }
        }
    }
}
