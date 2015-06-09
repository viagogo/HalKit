using HalKit.Models.Response;
using System;

namespace HalKit.Json
{
    /// <summary>
    /// When applied to the member of a type, specifies that the member
    /// should be serialized/deserialized into/from the "_links" section
    /// of "application/hal+json" content.
    /// </summary>
    /// <remarks>In general this should be applied to a property of type
    /// <see cref="Link"/> or <see cref="Link"/>[].</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RelAttribute : Attribute
    {
        private readonly string _rel;

        /// <summary>
        /// Initializes a new instances of a <see cref="RelAttribute"/>
        /// class with the given link relation.
        /// </summary>
        /// <param name="rel"></param>
        public RelAttribute(string rel)
        {
            Requires.ArgumentNotNull(rel, "rel");

            _rel = rel;
        }

        /// <summary>
        /// Gets the link relation of a link.
        /// </summary>
        public string Rel
        {
            get { return _rel; }
        }
    }
}
