using System.Collections.Generic;
using System.Net;

namespace HalKit.Http
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        private readonly Dictionary<string, string> _headers;

        public ApiResponse()
        {
            _headers = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Headers
        {
            get { return _headers; }
        }

        public HttpStatusCode StatusCode { get; set; }
        public string Body { get; set; }
        public T BodyAsObject { get; set; }
    }
}