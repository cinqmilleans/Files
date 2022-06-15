using Files.Shared.Cloud;
using Files.Shared.Extensions;
using Files.Uwp.Filesystem.Cloud.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Uwp.Filesystem.Cloud
{
    public class CloudProviderController : ICloudProviderDetector
    {
        public async IAsyncEnumerable<ICloudProvider> DetectAsync()
        {
            var tasks = new List<Task<List<ICloudProvider>>>();
            foreach (var detector in EnumerateDetectors())
            {
                tasks.Add(detector.DetectAsync().ToList());
            }
            await Task.WhenAll(tasks);

            var providers = tasks
                .SelectMany(task => task.Result)
                .OrderBy(task => task.ID.ToString())
                .ThenBy(task => task.Name)
                .Distinct();

            foreach (var provider in providers)
            {
                yield return provider;
            }
        }

        private static IEnumerable<ICloudProviderDetector> EnumerateDetectors()
            => new List<ICloudProviderDetector>
            {
                new GoogleDriveCloudProvider(),
                new DropBoxCloudProvider(),
                new BoxCloudProvider(),
                new AppleCloudProvider(),
                new GenericCloudProvider(),
                new SynologyDriveCloudProvider(),
            }.Select(detector => new TryCloudProvider(detector));
    }
}