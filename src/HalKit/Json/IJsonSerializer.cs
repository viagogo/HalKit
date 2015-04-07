namespace HalKit.Json
{
    public interface IJsonSerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string json);
    }
}
