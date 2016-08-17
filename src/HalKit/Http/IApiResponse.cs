using System.Collections.Generic;
using System.Net;

namespace HalKit.Http
{
    public interface IApiResponse<T> : IApiResponse
    {
        T BodyAsObject { get; }
    }

    public interface IApiResponse
    {
        IDictionary<string, IEnumerable<string>> Headers { get; }
        HttpStatusCode StatusCode { get; }
        string Body { get; }
    }
}
