using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HalKit.Json
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings
            = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new[] { new ResourceConverter() }
            };

        public Task<string> SerializeAsync(object value)
        {
            return Task.Factory.StartNew(
                () => JsonConvert.SerializeObject(value, Settings));
        }

        public Task<T> DeserializeAsync<T>(string json)
        {
            return Task.Factory.StartNew(
                () => JsonConvert.DeserializeObject<T>(json, Settings));
        }
    }
}
