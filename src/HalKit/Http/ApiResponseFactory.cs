using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Json;

namespace HalKit.Http
{
    public class ApiResponseFactory : IApiResponseFactory
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IHalKitConfiguration _configuration;

        public ApiResponseFactory(IJsonSerializer jsonSerializer, IHalKitConfiguration configuration)
        {
            Requires.ArgumentNotNull(jsonSerializer, nameof(jsonSerializer));
            Requires.ArgumentNotNull(configuration, nameof(configuration));

            _jsonSerializer = jsonSerializer;
            _configuration = configuration;
        }

        public async Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response)
        {
            string body = null;
            object bodyAsObject = null;
            var responseHttpHeaders = response.Headers.ToList();
            using (var content = response.Content)
            {
                if (content != null)
                {
                    if (typeof(T) != typeof(byte[]))
                    {
                        body = await response.Content.ReadAsStringAsync().ConfigureAwait(_configuration);
                        if (body != null && 
                            IsJsonContent(response.Content) && 
                            response.IsSuccessStatusCode)
                        {
                            bodyAsObject = _jsonSerializer.Deserialize<T>(body);
                        }
                    }
                    else
                    {
                        bodyAsObject = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(_configuration);
                    }

                    responseHttpHeaders.AddRange(content.Headers);
                }
            }

            var apiResponse = new ApiResponse<T>
            {
                StatusCode = response.StatusCode,
                Body = body,
                BodyAsObject = (T) bodyAsObject,
                Headers = responseHttpHeaders
                            .GroupBy(kv => kv.Key, kv => kv.Value)
                            .ToDictionary(grp => grp.Key, grp => grp.SelectMany(i => i.ToList()).ToList().AsEnumerable())
            };

            return apiResponse;
        }

        private bool IsJsonContent(HttpContent content)
        {
            if (content.Headers.ContentType == null)
            {
                return false;
            }

            return content.Headers.ContentType.MediaType == "application/hal+json" ||
                   content.Headers.ContentType.MediaType == "application/json" ||
                   content.Headers.ContentType.MediaType == "text/json";
        }
    }
}
