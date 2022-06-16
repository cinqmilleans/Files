using Files.Shared.Cloud;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Files.Uwp.Filesystem.Cloud
{
    public interface ICloudDetector
    {
        Task<IEnumerable<ICloudProvider>> DetectCloudProvidersAsync();
    }
}