using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HalKit.Models.Response;
using Tavis.UriTemplates;

namespace HalKit.Services
{
    public class LinkResolver : ILinkResolver
    {
        public Uri ResolveLink(Uri rootEndpoint, Link link, IDictionary<string, string> parameters)
        {
            Requires.ArgumentNotNull(link, nameof(link));

            if (!link.HRef.Contains(rootEndpoint.AbsoluteUri))
            {
                // Make new href from relative path (using absolute path as base)
                link.HRef = rootEndpoint + link.HRef.Replace(rootEndpoint.AbsolutePath, "");
            }

            return ResolveLink(link, parameters);
        }

        public Uri ResolveLink(Link link, IDictionary<string, string> parameters)
        {
            Requires.ArgumentNotNull(link, nameof(link));

            if (parameters == null || !parameters.Any())
            {
                return new Uri(link.HRef);
            }

            if (!link.IsTemplated)
            {
                return AppendParametersAsQueryParams(link.HRef, parameters);
            }

            var templateParameters = parameters.ToDictionary(kv => kv.Key, kv => (object) kv.Value);
            var resolvedUrl = new UriTemplate(link.HRef).AddParameters(templateParameters).Resolve();
            return new Uri(resolvedUrl);
        }

        private Uri AppendParametersAsQueryParams(
            string href,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var uriBuilder = new UriBuilder(href);
            var existingQueryParameters = uriBuilder.Query.Replace("?", "");
            if (!string.IsNullOrEmpty(existingQueryParameters) && !existingQueryParameters.EndsWith("&"))
            {
                existingQueryParameters += "&";
            }

            var parametersWithValues = parameters.Where(kv => !string.IsNullOrEmpty(kv.Value));
            var parametersQueryString = string.Join("&", parametersWithValues.Select(kv => kv.Key + "=" + Uri.EscapeDataString(kv.Value)));
            uriBuilder.Query = existingQueryParameters + parametersQueryString;
            return uriBuilder.Uri;
        }
    }
}