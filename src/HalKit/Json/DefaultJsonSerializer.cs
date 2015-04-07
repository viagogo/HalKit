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

        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }
}
