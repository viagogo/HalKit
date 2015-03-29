using System;
using System.Collections.Generic;
using HalKit.Models;

namespace HalKit.Services
{
    public interface ILinkResolver
    {
        Uri ResolveLink(Link link, IDictionary<string, string> parameters);
    }
}
