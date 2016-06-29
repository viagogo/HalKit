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
                                    DateParseHandling = DateParseHandling.None,
                                    NullValueHandling = NullValueHandling.Ignore,
                                };
                    _settings.ContractResolver = new ResourceContractResolver(_settings);
                }

                return _settings;
            }
        }

        public string Serialize(object value) => JsonConvert.SerializeObject(value, Settings);

        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);
    }
}
