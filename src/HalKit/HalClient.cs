using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HalKit.Http;
using HalKit.Models;
using HalKit.Resources;
using HalKit.Services;

namespace HalKit
{
    public class HalClient : IHalClient
    {
        private const string HalJsonMediaType = "application/hal+json";

        private readonly IHttpConnection _httpConnection;
        private readonly IHalKitConfiguration _configuration;
        private readonly ILinkResolver _linkResolver;

        public HalClient(IHttpConnection httpConnection,
                         IHalKitConfiguration configuration,
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

        public Task<RootResource> GetRootAsync()
        {
            return GetRootAsync(new Dictionary<string, string>());
        }

        public Task<RootResource> GetRootAsync(IDictionary<string, string> parameters)
        {
            return GetRootAsync(parameters, new Dictionary<string, IEnumerable<string>>());
        }

        public Task<RootResource> GetRootAsync(
            IDictionary<string, string> parameters,
            IDictionary<string, IEnumerable<string>> headers)
        {
            return GetAsync<RootResource>(
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
            Requires.ArgumentNotNull(link, "link");
            Requires.ArgumentNotNull(method, "method");

            headers = headers ?? new Dictionary<string, IEnumerable<string>>();
            if (!headers.ContainsKey("Accept"))
            {
                headers.Add("Accept", new[] { HalJsonMediaType });
            }

            var response = await _httpConnection.SendRequestAsync<T>(
                                    _linkResolver.ResolveLink(link, parameters),
                                    method,
                                    body,
                                    headers).ConfigureAwait(_configuration);
            return response.BodyAsObject;
        }
    }
}