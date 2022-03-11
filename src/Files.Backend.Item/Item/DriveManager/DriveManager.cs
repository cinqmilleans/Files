using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;
using WatcherStatus = Windows.Devices.Enumeration.DeviceWatcherStatus;

namespace Files.Backend.Item
{
    public class DriveManager : ObservableObject, IDriveManager
    {
        private readonly ILogger? logger = Ioc.Default.GetService<ILogger>();

        private readonly DeviceWatcher watcher;

        private readonly ObservableCollection<IDriveItem> drives = new();
        public ReadOnlyObservableCollection<IDriveItem> Drives { get; }

        public DriveManager(ILogger logger)
        {
            this.logger = logger;

            Drives = new(drives);

            watcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());
            watcher.Added += DeviceAdded;
            watcher.Removed += DeviceRemoved;
            watcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
        }

        public void Dispose()
        {
            if (watcher.Status is WatcherStatus.Started or WatcherStatus.EnumerationCompleted)
            {
                watcher.Stop();
            }
        }

        private IEnumerable<DriveItem> GetDrives()
        {
            lock (drives)
            {
                return drives.Cast<DriveItem>();
            }
        }

        private void StartDeviceWatcher()
        {
            if (watcher.Status is WatcherStatus.Created or WatcherStatus.Stopped or WatcherStatus.Aborted)
            {
                watcher?.Start();
            }
        }

        private void DeviceAdded(DeviceWatcher _, DeviceInformation info)
        {
            try
            {
                string deviceID = info.Id;
                var root = StorageDevice.FromId(deviceID);
                var driveItem = new DriveItem(root, deviceID);

                lock (drives)
                {
                    bool isIteminList = drives.Cast<DriveItem>().Any(item => item.DeviceID == deviceID || IsEqual(item, root));
                    if (!isIteminList)
                    {
                        logger?.Info($"Drive added: {driveItem.Path}, {driveItem.DriveType}");
                        drives.Add(driveItem);
                    }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or ArgumentException)
            {
                logger?.Warn($"{ex.GetType()}: Attempting to add the device, {info.Name}, "
                   + $"failed at the StorageFolder initialization step. This device will be ignored. Device ID: {info.Id}");
            }

            static bool IsEqual(IDriveItem item, StorageFolder root)
                => string.IsNullOrEmpty(root.Path)
                    ? item.Path.Contains(root.Name, StringComparison.OrdinalIgnoreCase)
                    : item.Path == root.Path;
        }

        private void DeviceRemoved(DeviceWatcher _, DeviceInformationUpdate info)
        {
            string deviceID = info.Id;
            logger?.Info($"Drive removed: {deviceID}");
            GetDrives().ToList().RemoveAll(x => x.DeviceID == deviceID);
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Debug.WriteLine("DriveWatcher_EnumerationCompleted");
        }

        /*private async Task UpdateDrivesAsync()
        {
            // Flag set if any drive throws UnauthorizedAccessException
            bool unauthorizedAccessDetected = false;
            var list = DriveInfo.GetDrives().ToList();
            foreach (var drive in list)
            {
                var res = await FilesystemTasks.Wrap(() => StorageFolder.GetFolderFromPathAsync(drive.Name).AsTask());
                if (res == FileSystemStatusCode.Unauthorized)
                {
                    unauthorizedAccessDetected = true;
                    Logger.Warn($"{res.ErrorCode}: Attempting to add the device, {drive.Name}, failed at the StorageFolder initialization step. This device will be ignored.");
                    continue;
                }
                else if (!res)
                {
                    Logger.Warn($"{res.ErrorCode}: Attempting to add the device, {drive.Name}, failed at the StorageFolder initialization step. This device will be ignored.");
                    continue;
                }
                using var thumbnail = (StorageItemThumbnail)await FilesystemTasks.Wrap(() => res.Result.GetThumbnailAsync(ThumbnailMode.SingleItem, 40, ThumbnailOptions.UseCurrentScale).AsTask());
                var type = GetDriveType(drive);
                var driveItem = await DriveItem.CreateFromPropertiesAsync(res.Result, drive.Name.TrimEnd('\\'), type, thumbnail);
                lock (drivesList)
                {
                    // If drive already in list, skip.
                    if (drivesList.Any(x => x.Path == drive.Name))
                    {
                        continue;
                    }
                    Logger.Info($"Drive added: {driveItem.Path}, {driveItem.Type}");
                    drivesList.Add(driveItem);
                }
            }
            return unauthorizedAccessDetected;
        }
    }*/
    }
}
