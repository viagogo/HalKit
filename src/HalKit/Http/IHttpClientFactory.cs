using System.Collections.Generic;
using System.Net.Http;

namespace HalKit.Http
{
    public interface IHttpClientFactory
    {
        HttpClient CreateClient(IEnumerable<DelegatingHandler> handlers);
    }
}