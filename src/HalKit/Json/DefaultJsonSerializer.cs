using Newtonsoft.Json;

namespace HalKit.Json
{
    public class DefaultJsonSerializer : IJsonSerializer
    {
        private static JsonSerializerSettings _settings;
        
        private static JsonSerializerSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new JsonSerializerSettings
                                {
                                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                    NullValueHandling = NullValueHandling.Ignore,
                                };
                    _settings.ContractResolver = new ResourceContractResolver(_settings);
                }

                return _settings;
            }
        }

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
