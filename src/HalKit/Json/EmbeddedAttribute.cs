using System;

namespace HalKit.Json
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmbeddedAttribute : Attribute
    {
        private readonly string _rel;

        public EmbeddedAttribute(string rel)
        {
            _rel = rel;
        }

        public string Rel
        {
            get { return _rel; }
        }
    }
}
