using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Http;
using HalKit.Models.Request;
using HalKit.Models.Response;
using HalKit.Services;

namespace HalKit
{
    public class HalClient : IHalClient
    {
        private const string HalJsonMediaType = "application/hal+json";

        private readonly IHttpConnection _httpConnection;
        private readonly IHalKitConfiguration _configuration;
        private readonly ILinkResolver _linkResolver;

        public HalClient(IHalKitConfiguration configuration)
            : this(configuration, new HttpConnection(new DelegatingHandler[] { }, configuration))
        {
        }

        public HalClient(IHalKitConfiguration configuration, IHttpConnection httpConnection)
            : this(configuration, httpConnection, new LinkResolver())
        {
        }

        public HalClient(IHalKitConfiguration configuration,
                         IHttpConnection httpConnection,
                         ILinkResolver linkResolver)
        {
            Requires.ArgumentNotNull(httpConnection, "httpConnection");
            Requires.ArgumentNotNull(configuration, "configuration");
            Requires.ArgumentNotNull(linkResolver, "linkResolver");
            if (configuration.RootEndpoint == null)
            {
                throw new ArgumentException("configuration must have a RootEndpoint");
            }

            _httpConnection = httpConnection;
            _configuration = configuration;
            _linkResolver = linkResolver;
        }

        public Task<TRootResource> GetRootAsync<TRootResource>()
            where TRootResource : Resource
        {
            return GetRootAsync<TRootResource>(new Dictionary<string, string>());
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(IDictionary<string, string> parameters)
            where TRootResource : Resource
        {
            return GetRootAsync<TRootResource>(parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(IRequestParameters request)
            where TRootResource : Resource
        {
            Requires.ArgumentNotNull(request, "request");

            return GetRootAsync<TRootResource>(request.Parameters, request.Headers);
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
            where TRootResource : Resource
        {
            return GetAsync<TRootResource>(
                new Link {HRef = _configuration.RootEndpoint.OriginalString},
                parameters,
                headers);
        }

        public Task<T> GetAsync<T>(Link link)
        {
            return GetAsync<T>(link, new Dictionary<string, string>());
        }

        public Task<T> GetAsync<T>(Link link, IDictionary<string, string> parameters)
        {
            return GetAsync<T>(link, parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<T> GetAsync<T>(Link link, IRequestParameters request)
        {
            Requires.ArgumentNotNull(request, "request");

            return GetAsync<T>(link, request.Parameters, request.Headers);
        }

        public Task<T> GetAsync<T>(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Get,
                null,
                parameters,
                headers);
        }

        public Task<T> PostAsync<T>(Link link, object body)
        {
            return PostAsync<T>(link, body, new Dictionary<string, string>());
        }

        public Task<T> PostAsync<T>(Link link, object body, IDictionary<string, string> parameters)
        {
            return PostAsync<T>(link, body, parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<T> PostAsync<T>(Link link, object body, IRequestParameters request)
        {
            Requires.ArgumentNotNull(request, "request");

            return PostAsync<T>(link, body, request.Parameters, request.Headers);
        }

        public Task<T> PostAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Post,
                body,
                parameters,
                headers);
        }

        public Task<T> PutAsync<T>(Link link, object body)
        {
            return PutAsync<T>(link, body, new Dictionary<string, string>());
        }

        public Task<T> PutAsync<T>(Link link, object body, IDictionary<string, string> parameters)
        {
            return PutAsync<T>(link, body, parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<T> PutAsync<T>(Link link, object body, IRequestParameters request)
        {
            Requires.ArgumentNotNull(request, "request");

            return PutAsync<T>(link, body, request.Parameters, request.Headers);
        }

        public Task<T> PutAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Put,
                body,
                parameters,
                headers);
        }

        public Task<T> PatchAsync<T>(Link link, object body)
        {
            return PatchAsync<T>(link, body, new Dictionary<string, string>());
        }

        public Task<T> PatchAsync<T>(Link link, object body, IDictionary<string, string> parameters)
        {
            return PatchAsync<T>(link, body, parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<T> PatchAsync<T>(Link link, object body, IRequestParameters request)
        {
            Requires.ArgumentNotNull(request, "request");

            return PatchAsync<T>(link, body, request.Parameters, request.Headers);
        }

        public Task<T> PatchAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                new HttpMethod("Patch"),
                body,
                parameters,
                headers);
        }

        public Task<IApiResponse> DeleteAsync(Link link)
        {
            return DeleteAsync(link, new Dictionary<string, string>());
        }

        public Task<IApiResponse> DeleteAsync(Link link, IDictionary<string, string> parameters)
        {
            return DeleteAsync(link, parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<IApiResponse> DeleteAsync(Link link, IRequestParameters request)
        {
            Requires.ArgumentNotNull(request, "request");

            return DeleteAsync(link, request.Parameters, request.Headers);
        }

        public async Task<IApiResponse> DeleteAsync(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            var response = await SendRequestAsync<object>(
                            link,
                            HttpMethod.Delete,
                            null,
                            parameters,
                            headers);
            return response;
        }

        public IHalKitConfiguration Configuration
        {
            get { return _configuration; }
        }

        public IHttpConnection HttpConnection
        {
            get { return _httpConnection; }
        }

        private async Task<T> SendRequestAndGetBodyAsync<T>(
            Link link,
            HttpMethod method,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            var response = await SendRequestAsync<T>(link, method, body, parameters, headers);
            return response.BodyAsObject;
        }

        private async Task<IApiResponse<T>> SendRequestAsync<T>(
            Link link,
            HttpMethod method,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            Requires.ArgumentNotNull(link, "link");
            Requires.ArgumentNotNull(method, "method");

            headers = headers ?? new Dictionary<string, IEnumerable<string>>();
            if (!headers.ContainsKey("Accept"))
            {
                headers.Add("Accept", new[] { HalJsonMediaType });
            }

            return await _httpConnection.SendRequestAsync<T>(
                        _linkResolver.ResolveLink(link, parameters),
                        method,
                        body,
                        headers).ConfigureAwait(_configuration);
        }
    }
}