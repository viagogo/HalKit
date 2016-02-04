using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Http;

namespace HalKit.Tests.Fakes
{
    public class FakeHttpConnection : IHttpConnection
    {
        private readonly IApiResponse _response;

        public FakeHttpConnection(IApiResponse response = null, IHalKitConfiguration config = null)
        {
            _response = response;
            Configuration = config ?? new HalKitConfiguration(new Uri("http://foo.api.com"));
        }

        public Task<IApiResponse<T>> SendRequestAsync<T>(Uri uri, HttpMethod method, object body, IDictionary<string, IEnumerable<string>> headers)
        {
            var response = _response as IApiResponse<T>;
            return Task.FromResult(response ?? new ApiResponse<T>());
        }

        public Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest apiRequest, IDictionary<string, IEnumerable<string>> headers)
        {
            return SendRequestAsync<T>(apiRequest.Uri, apiRequest.Method, apiRequest.Body, headers);
        }

        public IHalKitConfiguration Configuration { get; private set; }
        public HttpClient Client { get; }
    }
}
