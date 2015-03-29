using System.Net.Http;
using System.Threading.Tasks;

namespace HalKit.Http
{
    public interface IApiResponseFactory
    {
        Task<IApiResponse<T>> CreateApiResponseAsync<T>(HttpResponseMessage response);
    }
}
