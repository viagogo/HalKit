using System.Runtime.Serialization;

namespace HalKit.Models.Response
{
    /// <summary>
    /// A <see cref="Resource"/> that acts as the "Home Page" of a Hypermedia-driven
    /// API. This resource provides links to other API resources.
    /// </summary>
    [DataContract]
    public class RootResource : Resource
    {
    }
}
