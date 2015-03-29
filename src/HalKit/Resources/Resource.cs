using System.Runtime.Serialization;
using HalKit.Models;

namespace HalKit.Resources
{
    [DataContract]
    public class Resource
    {
        [IgnoreDataMember]
        public LinkCollection Links { get; set; }
    }
}
