using System.Runtime.Serialization;

namespace HalKit.Models
{
    [DataContract]
    public class CurieLink : Link
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
