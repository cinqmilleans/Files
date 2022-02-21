using System;
using Windows.Devices.Portable;
using Windows.Storage;

namespace Files.BackEnd
{
    internal interface IStorageFolderBuilder
    {
        StorageFolder BuildFromDeviceId(string deviceId);
    }

    internal class StorageFolderBuilder : IStorageFolderBuilder
    {
        public StorageFolder BuildFromDeviceId(string deviceId)
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
    }
}
