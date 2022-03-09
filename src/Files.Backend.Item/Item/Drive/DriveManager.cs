﻿using System.Collections.Generic;

namespace Files.Backend.Item
{
    public interface IDriveManager
    {
        IReadOnlyList<IDriveItem> Drives { get; }
    }

    /*public class DriveManager : IDriveManager
    {
        private DeviceWatcher watcher;

        private readonly List<DriveItem> drives = new();
        public IReadOnlyList<IDriveItem> Drives
        {
            get
            {
                lock (drives)
                {
                    return drives.AsReadOnly();
                }
            }
        }

        public DriveManager()
        {
            watcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());
            watcher.Added += DeviceAdded;
            watcher.Removed += DeviceRemoved;
            watcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
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

    private void DeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
        }

        private void DeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Debug.WriteLine("DriveWatcher_EnumerationCompleted");
        }

        public static StorageFolder BuildFromDeviceId(string deviceId)
        {
            try
            {
                return StorageDevice.FromId(deviceId);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"The storage folder cannot be created. Device ID: {deviceId}", e);
            }
        }
    }*/
}
