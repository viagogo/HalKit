using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HalKit.Http
{
    public interface IHttpConnection
    {
        Task<IApiResponse<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            object body,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken);

        Task<IApiResponse<T>> SendRequestAsync<T>(IApiRequest apiRequest, CancellationToken cancellationToken);

        IHalKitConfiguration Configuration { get; }
        HttpClient Client { get; }
    }
}
