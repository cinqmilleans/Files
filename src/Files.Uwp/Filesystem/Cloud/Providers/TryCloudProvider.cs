using Files.Shared.Cloud;
using System.Collections.Generic;

namespace Files.Uwp.Filesystem.Cloud.Providers
{
    public class TryCloudProvider : ICloudProviderDetector
    {
        private readonly ICloudProviderDetector detector;

        public TryCloudProvider(ICloudProviderDetector detector) => this.detector = detector;

        public async IAsyncEnumerable<ICloudProvider> DetectAsync()
        {
            try
            {
                await foreach (var provider in detector.DetectAsync())
                {
                    yield return provider;
                }
            }
            finally {}
        }
    }
}