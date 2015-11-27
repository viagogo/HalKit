using System;

namespace HalKit.Json
{
    /// <summary>
    /// When applied to the member of a type, specifies that the member
    /// should be serialized/deserialized into/from the "_embedded" section
    /// of "application/hal+json" content.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmbeddedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instances of a <see cref="EmbeddedAttribute"/>
        /// class with the given link relation.
        /// </summary>
        /// <param name="rel"></param>
        public EmbeddedAttribute(string rel)
        {
            Requires.ArgumentNotNull(rel, nameof(rel));

            Rel = rel;
        }

        /// <summary>
        /// Gets the link relation of an embedded resource.
        /// </summary>
        public string Rel { get; }
    }
}
