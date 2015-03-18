using System.Threading;

namespace HalKit
{
    public interface IHalKitConfiguration
    {
        /// <summary>
        /// Determines whether asynchronous operations should capture the current
        /// <see cref="SynchronizationContext"/>.
        /// </summary>
        /// <remarks>See http://blog.stephencleary.com/2012/02/async-and-await.html#avoiding-context.</remarks>
        bool CaptureSynchronizationContext { get; set; }
    }
}
