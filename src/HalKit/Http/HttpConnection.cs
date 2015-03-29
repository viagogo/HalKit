using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HalKit.Json;

namespace HalKit.Http
{
    public class HttpConnection : IHttpConnection
    {
        private readonly HttpClient _httpClient;
        private readonly IHalKitConfiguration _configuration;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IApiResponseFactory _responseFactory;

        public HttpConnection(IEnumerable<DelegatingHandler> handlers,
                              IHalKitConfiguration configuration)
            : this(handlers,
                   configuration,
                   new HttpClientFactory(),
                   new DefaultJsonSerializer(),
                   new ApiResponseFactory(new DefaultJsonSerializer(), configuration))
        {
        }

        public HttpConnection(IEnumerable<DelegatingHandler> handlers,
                              IHalKitConfiguration configuration,
                              IHttpClientFactory httpClientFactory,
                              IJsonSerializer jsonSerializer,
                              IApiResponseFactory responseFactory)
        {
            Requires.ArgumentNotNull(handlers, "handlers");
            Requires.ArgumentNotNull(configuration, "configuration");
            Requires.ArgumentNotNull(httpClientFactory, "httpClientFactory");
            Requires.ArgumentNotNull(jsonSerializer, "jsonSerializer");
            Requires.ArgumentNotNull(responseFactory, "responseFactory");

            _httpClient = httpClientFactory.CreateClient(handlers);
            _configuration = configuration;
            _jsonSerializer = jsonSerializer;
            _responseFactory = responseFactory;
        }

        public async Task<IApiResponse<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            object body,
            IDictionary<string, IEnumerable<string>> headers)
        {
            Requires.ArgumentNotNull(uri, "uri");
            Requires.ArgumentNotNull(method, "method");

            using (var request = new HttpRequestMessage {RequestUri = uri, Method = method})
            {
                string contentType = null;
                headers = headers ?? new Dictionary<string, IEnumerable<string>>();
                foreach (var header in headers)
                {
                    if (header.Key == "Content-Type")
                    {
                        contentType = header.Value.FirstOrDefault();
                        continue;
                    }

                    request.Headers.Add(header.Key, header.Value);
                }
                request.Content = await GetRequestContentAsync(method, body, contentType);

                var responseMessage = await _httpClient.SendAsync(request, CancellationToken.None).ConfigureAwait(_configuration);
                return await _responseFactory.CreateApiResponseAsync<T>(responseMessage).ConfigureAwait(_configuration);
            }
        }

        private async Task<HttpContent> GetRequestContentAsync(
            HttpMethod method,
            object body,
            string contentType)
        {
            if (method == HttpMethod.Get || body == null)
            {
                return null;
            }

            if (body is HttpContent)
            {
                return body as HttpContent;
            }

            var bodyString = body as string;
            if (bodyString != null)
            {
                return new StringContent(bodyString, Encoding.UTF8, contentType);
            }

            var bodyStream = body as Stream;
            if (bodyStream != null)
            {
                var streamContent = new StreamContent(bodyStream);
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                return streamContent;
            }

            // Anything else gets serialized to JSON
            var bodyJson = await _jsonSerializer.SerializeAsync(body).ConfigureAwait(_configuration);
            return new StringContent(bodyJson, Encoding.UTF8, contentType);
        }

        public IHalKitConfiguration Configuration
        {
            get { return _configuration; }
        }
    }
}