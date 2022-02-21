using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Storage;

namespace Files.BackEnd
{
    internal interface IDriveProvider
    {
        DriveUpdateItems UpdateItem { get; }

        Task<DriveItem> Build(DeviceInformation information);
    }

    [Flags]
    internal enum DriveProviderOptions : ushort
    {
        None = 0x0000,
        LoadName = 0x0001,
        LoadSpaces = 0x0002,
        LoadIcons = 0x0004,
        LoadAll = LoadName + LoadSpaces + LoadIcons,
    };

    internal class DriveProvider : IDriveProvider
    {
        private readonly IStorageFolderBuilder folderBuilder = new StorageFolderBuilder();
        private readonly IDriveTypeConverter typeConverter = new DriveTypeConverter();

        public DriveUpdateItems UpdateItem { get; } = DriveUpdateItems.Name;

        public async Task<DriveItem> Build(DeviceInformation information)
        {
            var root = folderBuilder.BuildFromDeviceId(information.Id);

            var drive = new DriveItem(root)
            {
                Path = ToPath(root),
                DriveType = typeConverter.ToDriveType(root.Path),
            };

            await drive.UpdateAsync(UpdateItem);

            return drive;
        }

        private static string ToPath(StorageFolder root)
            => string.IsNullOrEmpty(root.Path) ? $"\\\\?\\{root.Name}\\" : root.Path;
    }

    internal class UnauthorizedAccessException : Exception
    {
        public UnauthorizedAccessException() : base() {}
        public UnauthorizedAccessException(string message) : base(message) {}
        public UnauthorizedAccessException(string message, Exception innerException) : base(message, innerException) {}
        protected UnauthorizedAccessException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}
