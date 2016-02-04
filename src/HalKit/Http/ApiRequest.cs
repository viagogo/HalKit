using System;
using System.Net.Http;

namespace HalKit.Http
{
    public class ApiRequest : IApiRequest
    {
        public Uri Uri { get; set; }
        public HttpMethod Method { get; set; }
        public object Body { get; set; }
    }
}
