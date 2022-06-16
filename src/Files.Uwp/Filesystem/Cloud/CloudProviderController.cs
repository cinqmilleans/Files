using Files.Shared.Cloud;
using Files.Uwp.Filesystem.Cloud.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Uwp.Filesystem.Cloud
{
    public class CloudProviderController : ICloudProviderDetector
    {
        public async Task<IEnumerable<ICloudProvider>> DetectAsync()
        {
            var tasks = new List<Task<IEnumerable<ICloudProvider>>>();
            foreach (var detector in EnumerateDetectors())
            {
                tasks.Add(detector.DetectAsync());
            }
            await Task.WhenAll(tasks);

            return tasks
                .SelectMany(task => task.Result)
                .OrderBy(task => task.ID.ToString())
                .ThenBy(task => task.Name)
                .Distinct();
        }

        private static IEnumerable<ICloudProviderDetector> EnumerateDetectors() => new List<ICloudProviderDetector>
        {
            new GoogleDriveCloudProvider(),
            new DropBoxCloudProvider(),
            new BoxCloudProvider(),
            new AppleCloudProvider(),
            new GenericCloudProvider(),
            new SynologyDriveCloudProvider(),
        };
    }
}