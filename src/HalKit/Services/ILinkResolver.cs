using System;
using System.Collections.Generic;
using HalKit.Models;

namespace HalKit.Services
{
    /// <summary>
    /// Applies parameters to <see cref="Link"/>s to produce <see cref="Uri"/>s.
    /// </summary>
    public interface ILinkResolver
    {
        /// <summary>
        /// Resolves the href of the given <see cref="Link"/> with the given parameters to
        /// get a <see cref="Uri"/>. If <paramref name="link"/> is templated then the parameters
        /// are applied in accordance with Uri Template Spec RFC6570 (http://tools.ietf.org/html/rfc6570).
        /// Otherwise, the parameters are added to the href as query parameters.
        /// </summary>
        Uri ResolveLink(Link link, IDictionary<string, string> parameters);
    }
}
