using CommunityToolkit.Mvvm.ComponentModel;
using Files.Backend.Item.Extension;
using Files.Shared;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;

namespace Files.Backend.Item
{
    internal class DriveItem : ObservableObject, IDriveItem
    {
        private readonly StorageFolder root;

        public string Path { get; init; } = string.Empty;

        private string name = string.Empty;
        public string Name
        {
            get => name;
            private set => SetProperty(ref name, value);
        }

        public DriveTypes DriveType { get; init; } = DriveTypes.Unknown;

        private ByteSize usedSpace = ByteSize.Zero;
        public ByteSize UsedSpace
        {
            get => usedSpace;
            private set => SetProperty(ref usedSpace, value);
        }

        private ByteSize freeSpace = ByteSize.Zero;
        public ByteSize FreeSpace
        {
            get => freeSpace;
            private set => SetProperty(ref freeSpace, value);
        }

        private ByteSize totalSpace = ByteSize.Zero;
        public ByteSize TotalSpace
        {
            get => totalSpace;
            private set => SetProperty(ref totalSpace, value);
        }

        public Uri? ImageSource { get; }
        public byte[]? ImageBytes { get; private set; }

        /*public async DriveItem Create(string name)
        {
            var root = await StorageFolder.GetFolderFromPathAsync(name);
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
        }*/
        public DriveItem(StorageFolder root)
        {
            this.root = root;
        }

        public async Task UpdateNameAsync()
            => Name = await root.GetPropertyAsync<string>("System.ItemNameDisplay") ?? string.Empty;

        public async Task UpdateSpaceAsync()
        {
            try
            {
                var properties = await root.GetPropertiesAsync<long>("System.Capacity", "System.Capacity");

                TotalSpace = properties["System.Capacity"];
                FreeSpace = properties["System.FreeSpace"];
                UsedSpace = FreeSpace <= TotalSpace ? TotalSpace - FreeSpace : ByteSize.Zero;
            }
            catch
            {
                UsedSpace = FreeSpace = TotalSpace = ByteSize.Zero;
            }
        }

        public async Task UpdateImageAsync()
        {
            var stream = await root.GetThumbnailAsync(ThumbnailMode.SingleItem, requestedSize: 40, ThumbnailOptions.UseCurrentScale);
            ImageBytes = await stream.ToByteArrayAsync();
        }
    }

    public class ItemException : Exception
    {
        public ItemErrors Error { get; } = ItemErrors.Unknown;

        public ItemException() {}
        public ItemException(string? message) : base(message) {}
        public ItemException(string? message, Exception innerException) : base(message, innerException)
            => Error = GetError(innerException);

        private static ItemErrors GetError(Exception e)
        {
            return (e, (uint)e.HResult) switch
            {
                (UnauthorizedAccessException, _) => ItemErrors.Unauthorized,
                (FileNotFoundException, _) => ItemErrors.NotFound, // Item was deleted
                (COMException, _) => ItemErrors.NotFound, // Item's drive was ejected
                (_, 0x8007000F) => ItemErrors.NotFound, // The system cannot find the drive specified
                (PathTooLongException, _) => ItemErrors.NameTooLong,
                (IOException, _) => ItemErrors.InUse,
                (ArgumentException, _) => ItemErrors.Unknown, // Item was invalid
                (_, 0x800700B7) => ItemErrors.AlreadyExists,
                _ => ItemErrors.Unknown,
            };
        }

    }

    public enum ItemErrors : ushort
    {
        Unknown,
        Unauthorized,
        NotFound,
        InUse,
        NameTooLong,
        AlreadyExists,
        InProgress,
    }
}
