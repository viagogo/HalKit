using System.Collections.Generic;
using System.Net;

namespace HalKit.Http
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        public ApiResponse()
        {
            Headers = new Dictionary<string, IEnumerable<string>>();
        }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }
        public T BodyAsObject { get; set; }
    }
}