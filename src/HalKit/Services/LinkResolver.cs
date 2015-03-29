using System;
using System.Collections.Generic;
using System.Linq;
using HalKit.Models;

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

            // TODO: Support Uri Templates
            return new Uri(link.HRef);
        }
    }
}