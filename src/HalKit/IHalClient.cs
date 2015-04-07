using System.Collections.Generic;
using System.Threading.Tasks;
using HalKit.Http;
using HalKit.Models.Request;
using HalKit.Models.Response;

namespace HalKit
{
    public interface IHalClient
    {
        Task<RootResource> GetRootAsync();

        Task<RootResource> GetRootAsync(IDictionary<string, string> parameters);

        Task<RootResource> GetRootAsync(IRequestParameters request);

        Task<RootResource> GetRootAsync(
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        Task<T> GetAsync<T>(Link link);

        Task<T> GetAsync<T>(Link link, IDictionary<string, string> parameters);

        Task<T> GetAsync<T>(Link link, IRequestParameters request);

        Task<T> GetAsync<T>(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        Task<T> PostAsync<T>(Link link, object body);

        Task<T> PostAsync<T>(Link link, object body, IDictionary<string, string> parameters);

        Task<T> PostAsync<T>(
            Link link,
            object body,
            IRequestParameters request);

        Task<T> PostAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        Task<T> PutAsync<T>(Link link, object body);

        Task<T> PutAsync<T>(Link link, object body, IDictionary<string, string> parameters);

        Task<T> PutAsync<T>(
            Link link,
            object body,
            IRequestParameters request);

        Task<T> PutAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        Task<T> PatchAsync<T>(Link link, object body);

        Task<T> PatchAsync<T>(Link link, object body, IDictionary<string, string> parameters);

        Task<T> PatchAsync<T>(
            Link link,
            object body,
            IRequestParameters request);

        Task<T> PatchAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        Task<IApiResponse> DeleteAsync(Link link);

        Task<IApiResponse> DeleteAsync(Link link, IDictionary<string, string> parameters);

        Task<IApiResponse> DeleteAsync(Link link, IRequestParameters request);

        Task<IApiResponse> DeleteAsync(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers);

        IHalKitConfiguration Configuration { get; }

        IHttpConnection HttpConnection { get; }
    }
}