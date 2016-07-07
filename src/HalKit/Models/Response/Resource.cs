using HalKit.Json;
using System.Runtime.Serialization;

namespace HalKit.Models.Response
{
    /// <summary>
    /// Represents a resource in a Hyper-media API.
    /// </summary>
    [DataContract]
    public class Resource
    {
        /// <summary>
        /// A <see cref="Link"/> representing the URI of a <see cref="Resource"/>.
        /// </summary>
        [Rel("self")]
        public virtual Link SelfLink { get; set; }
    }
}
