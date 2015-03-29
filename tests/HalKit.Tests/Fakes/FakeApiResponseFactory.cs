using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Http;

namespace HalKit.Tests.Fakes
{
    public class FakeApiResponseFactory : IApiResponseFactory
    {
        private readonly object _resp;

        public FakeApiResponseFactory(object resp = null)
        {
            _resp = resp;
        }

        public Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response)
        {
            var apiResponse = _resp as IApiResponse<T>;
            return Task.FromResult(apiResponse ?? new ApiResponse<T>());
        }
    }
}
