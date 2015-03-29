using System.Runtime.Serialization;

namespace HalKit.Models
{
    [DataContract]
    public class Resource
    {
        [IgnoreDataMember]
        public LinkCollection Links { get; set; }
    }
}
