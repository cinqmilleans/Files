using Files.Shared.Cloud;
using System.Collections.Generic;

namespace Files.Uwp.Filesystem.Cloud
{
    public interface ICloudProviderDetector
    {
        IAsyncEnumerable<ICloudProvider> DetectAsync();
    }
}