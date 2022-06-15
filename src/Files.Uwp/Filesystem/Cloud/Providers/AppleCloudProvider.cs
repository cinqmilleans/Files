using Files.Shared.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace Files.Uwp.Filesystem.Cloud.Providers
{
    public class AppleCloudProvider : ICloudProviderDetector
    {
        public async IAsyncEnumerable<ICloudProvider> DetectAsync()
        {
            string userPath = UserDataPaths.GetDefault().Profile;
            string iCloudPath = Path.Combine(userPath, "iCloudDrive");
            var driveFolder = await StorageFolder.GetFolderFromPathAsync(iCloudPath);

            yield return new CloudProvider(CloudProviders.AppleCloud)
            {
                Name = "iCloud",
                SyncFolder = driveFolder.Path,
            };
        }
    }
}