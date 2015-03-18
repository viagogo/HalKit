using System.Threading.Tasks;

namespace HalKit.Json
{
    public interface IJsonSerializer
    {
        Task<string> SerializeAsync(object value);
        Task<T> DeserializeAsync<T>(string json);
    }
}
