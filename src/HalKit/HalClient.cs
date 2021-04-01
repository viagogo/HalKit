using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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
            Requires.ArgumentNotNull(httpConnection, nameof(httpConnection));
            Requires.ArgumentNotNull(configuration, nameof(configuration));
            Requires.ArgumentNotNull(linkResolver, nameof(linkResolver));
            if (configuration.RootEndpoint == null)
            {
                throw new ArgumentException($"{nameof(configuration)} must have a RootEndpoint");
            }

            HttpConnection = httpConnection;
            Configuration = configuration;
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
            return GetRootAsync<TRootResource>(request, CancellationToken.None);
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(
            IRequestParameters request,
            CancellationToken cancellationToken)
            where TRootResource : Resource
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return GetRootAsync<TRootResource>(request.Parameters, request.Headers, cancellationToken);
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
            where TRootResource : Resource
        {
            return GetRootAsync<TRootResource>(parameters, headers, CancellationToken.None);
        }

        public Task<TRootResource> GetRootAsync<TRootResource>(
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken) where TRootResource : Resource
        {
            return GetAsync<TRootResource>(
                new Link { HRef = Configuration.RootEndpoint.OriginalString },
                parameters,
                headers,
                cancellationToken);
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
            return GetAsync<T>(link, request, CancellationToken.None);
        }

        public Task<T> GetAsync<T>(Link link, IRequestParameters request, CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return GetAsync<T>(link, request.Parameters, request.Headers, cancellationToken);
        }

        public Task<T> GetAsync<T>(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return GetAsync<T>(link, parameters, headers, CancellationToken.None);
        }

        public Task<T> GetAsync<T>(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Get,
                null,
                parameters,
                headers,
                cancellationToken);
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
            return PostAsync<T>(link, body, request, CancellationToken.None);
        }

        public Task<T> PostAsync<T>(Link link, object body, IRequestParameters request, CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return PostAsync<T>(link, body, request.Parameters, request.Headers, cancellationToken);
        }

        public Task<T> PostAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return PostAsync<T>(link, body, parameters, headers, CancellationToken.None);
        }

        public Task<T> PostAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Post,
                body,
                parameters,
                headers,
                cancellationToken);
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
            return PutAsync<T>(link, body, request, CancellationToken.None);
        }

        public Task<T> PutAsync<T>(Link link, object body, IRequestParameters request, CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return PutAsync<T>(link, body, request.Parameters, request.Headers, cancellationToken);
        }

        public Task<T> PutAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return PutAsync<T>(link, body, parameters, headers, CancellationToken.None);
        }

        public Task<T> PutAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Put,
                body,
                parameters,
                headers,
                cancellationToken);
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
            return PatchAsync<T>(link, body, request, CancellationToken.None);
        }

        public Task<T> PatchAsync<T>(Link link, object body, IRequestParameters request, CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return PatchAsync<T>(link, body, request.Parameters, request.Headers, cancellationToken);
        }

        public Task<T> PatchAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return PatchAsync<T>(link, body, parameters, headers, CancellationToken.None);
        }

        public Task<T> PatchAsync<T>(
            Link link,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            return SendRequestAndGetBodyAsync<T>(
                link,
                HttpMethod.Patch,
                body,
                parameters,
                headers,
                cancellationToken);
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
            return DeleteAsync(link, request, CancellationToken.None);
        }

        public Task<IApiResponse> DeleteAsync(Link link, IRequestParameters request, CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(request, nameof(request));

            return DeleteAsync(link, request.Parameters, request.Headers, cancellationToken);
        }

        public Task<IApiResponse> DeleteAsync(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return DeleteAsync(link, parameters, headers, CancellationToken.None);
        }

        public async Task<IApiResponse> DeleteAsync(
            Link link,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            var response = await SendRequestAsync<object>(
                                    link,
                                    HttpMethod.Delete,
                                    null,
                                    parameters,
                                    headers,
                                    cancellationToken).ConfigureAwait(Configuration);
            return response;
        }

        public IHalKitConfiguration Configuration { get; }

        public IHttpConnection HttpConnection { get; }

        private async Task<T> SendRequestAndGetBodyAsync<T>(
            Link link,
            HttpMethod method,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            var response = await SendRequestAsync<T>(
                                    link,
                                    method,
                                    body,
                                    parameters,
                                    headers,
                                    cancellationToken).ConfigureAwait(Configuration);
            return response.BodyAsObject;
        }

        private async Task<IApiResponse<T>> SendRequestAsync<T>(
            Link link,
            HttpMethod method,
            object body,
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers,
            CancellationToken cancellationToken)
        {
            Requires.ArgumentNotNull(link, nameof(link));
            Requires.ArgumentNotNull(method, nameof(method));

            headers = headers ?? new Dictionary<string, IEnumerable<string>>();
            if (!headers.ContainsKey("Accept"))
            {
                headers.Add("Accept", new[] { HalJsonMediaType });
            }

            return await HttpConnection.SendRequestAsync<T>(
                        _linkResolver.ResolveLink(link, parameters),
                        method,
                        body,
                        headers,
                        cancellationToken).ConfigureAwait(Configuration);
        }
    }
}