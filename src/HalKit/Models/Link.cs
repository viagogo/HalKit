using System.Runtime.Serialization;

namespace HalKit.Models
{
    [DataContract]
    public class Link
    {
        [IgnoreDataMember]
        public string Rel { get; set; }

        [DataMember(Name = "href")]
        public string HRef { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "templated")]
        public bool Templated { get; set; }
    }
}
