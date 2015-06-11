using System.Runtime.Serialization;

namespace HalKit.Models.Response
{
    /// <summary>
    /// Represents a hyperlink between API resources.
    /// </summary>
    [DataContract]
    public class Link
    {
        /// <summary>
        /// Gets or sets the target URI of the <see cref="Link"/>.
        /// </summary>
        [DataMember(Name = "href")]
        public string HRef { get; set; }

        /// <summary>
        /// Gets or sets the title that describes the resource targeted by the
        /// <see cref="Link"/>.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value that signifies whether the <see cref="HRef"/>
        /// is templated.
        /// </summary>
        [DataMember(Name = "templated")]
        public bool IsTemplated { get; set; }
    }
}
