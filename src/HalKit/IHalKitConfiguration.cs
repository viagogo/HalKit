using System;
using System.Threading;
using HalKit.Models.Response;

namespace HalKit
{
    public interface IHalKitConfiguration
    {
        /// <summary>
        /// The <see cref="Uri"/> of the <see cref="Resource"/> of an API.
        /// This resource provides links to other API resources.
        /// </summary>
        Uri RootEndpoint { get; set; }

        /// <summary>
        /// Determines whether asynchronous operations should capture the current
        /// <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <remarks>See http://blog.stephencleary.com/2012/02/async-and-await.html#avoiding-context.</remarks>
        bool CaptureSynchronizationContext { get; set; }
    }
}
