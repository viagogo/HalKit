using System;
using System.Collections.Generic;
using System.Linq;
using HalKit.Models;
using Tavis.UriTemplates;

namespace HalKit.Services
{
    public class LinkResolver : ILinkResolver
    {
        public Uri ResolveLink(Link link, IDictionary<string, string> parameters)
        {
            Requires.ArgumentNotNull(link, "link");

            if (parameters == null || !parameters.Any())
            {
                return new Uri(link.HRef);
            }

            if (!link.IsTemplated)
            {
                return AppendParametersAsQueryParams(link, parameters);
            }

            var templateParameters = parameters.ToDictionary(kv => kv.Key, kv => (object) kv.Value);
            var resolvedUrl = new UriTemplate(link.HRef).AddParameters(templateParameters).Resolve();
            return new Uri(resolvedUrl);
        }

        private Uri AppendParametersAsQueryParams(
            Link link,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var uriBuilder = new UriBuilder(link.HRef);
            var existingQueryParameters = uriBuilder.Query.Replace("?", "");
            if (!string.IsNullOrEmpty(existingQueryParameters) && !existingQueryParameters.EndsWith("&"))
            {
                existingQueryParameters += "&";
            }

            var parametersQueryString = string.Join("&", parameters.Select(kv => kv.Key + "=" + kv.Value));
            uriBuilder.Query = existingQueryParameters + parametersQueryString;
            return uriBuilder.Uri;
        }
    }
}