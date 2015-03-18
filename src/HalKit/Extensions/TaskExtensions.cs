using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HalKit
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable<TResult> ConfigureAwait<TResult>(
            this Task<TResult> task,
            IHalKitConfiguration configuration)
        {
            return task.ConfigureAwait(configuration.CaptureSynchronizationContext);
        }
    }
}
