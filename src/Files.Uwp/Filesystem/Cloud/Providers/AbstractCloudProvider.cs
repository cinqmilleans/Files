using Files.Shared.Cloud;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Uwp.Filesystem.Cloud.Providers
{
    public abstract class AbstractCloudProvider : ICloudProviderDetector
    {
        public async Task<IEnumerable<ICloudProvider>> DetectAsync()
        {
            try
            {
                var providers = new List<ICloudProvider>();
                await foreach (var provider in GetProviders())
                {
                    providers.Add(provider);
                }
                return providers;
            }
            catch
            {
                return Enumerable.Empty<ICloudProvider>();
            }
        }

        protected abstract IAsyncEnumerable<ICloudProvider> GetProviders();
    }
}