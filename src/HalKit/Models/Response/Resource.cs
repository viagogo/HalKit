using System.Runtime.Serialization;

namespace HalKit.Models.Response
{
    [DataContract]
    public class Resource
    {
        [IgnoreDataMember]
        public LinkCollection Links { get; set; }
    }
}
