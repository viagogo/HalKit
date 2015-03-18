namespace HalKit
{
    public class HalKitConfiguration : IHalKitConfiguration
    {
        public static readonly HalKitConfiguration Default
            = new HalKitConfiguration {CaptureSynchronizationContext = false};

        public bool CaptureSynchronizationContext { get; set; }
    }
}
