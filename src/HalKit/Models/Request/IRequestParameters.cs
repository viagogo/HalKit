using System.Collections.Generic;

namespace HalKit.Models.Request
{
    /// <summary>
    /// Provides the query parameters and headers in a particular request.
    /// </summary>
    public interface IRequestParameters
    {
        /// <summary>
        /// The query parameters to be used in a request
        /// </summary>
        IDictionary<string, string> Parameters { get; }

        /// <summary>
        /// The headers to be used in a request
        /// </summary>
        IDictionary<string, IEnumerable<string>> Headers { get; }
    }
}
