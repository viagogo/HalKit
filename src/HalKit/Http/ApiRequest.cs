using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HalKit.Http
{
    public class ApiRequest : IApiRequest
    {
        public Uri Uri { get; set; }
        public HttpMethod Method { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public object Body { get; set; }
    }
}
