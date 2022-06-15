﻿using Files.Shared.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace Files.Uwp.Filesystem.Cloud.Providers
{
    public class BoxCloudProvider : ICloudProviderDetector
    {
        public async IAsyncEnumerable<ICloudProvider> DetectAsync()
        {
            string configPathBoxDrive = Path.Combine(UserDataPaths.GetDefault().LocalAppData, @"Box\Box\data\shell\sync_root_folder.txt");
            string configPathBoxSync = Path.Combine(UserDataPaths.GetDefault().LocalAppData, @"Box Sync\sync_root_folder.txt");

            StorageFile configFile = await FilesystemTasks.Wrap(() => StorageFile.GetFileFromPathAsync(configPathBoxDrive).AsTask());
            if (configFile is null)
            {
                configFile = await FilesystemTasks.Wrap(() => StorageFile.GetFileFromPathAsync(configPathBoxSync).AsTask());
            }
            if (configFile is not null)
            {
                string syncPath = await FileIO.ReadTextAsync(configFile);
                if (!string.IsNullOrEmpty(syncPath))
                {
                    yield return new CloudProvider(CloudProviders.Box)
                    {
                        Name = "Box",
                        SyncFolder = syncPath,
                    };
                }
            }
        }
    }
}