using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.BackEnd
{
    public interface IDriveItem : IItem
    {
        DriveTypes DriveType { get; }

        ByteSize UsedSpace { get; }
        ByteSize FreeSpace { get; }
        ByteSize TotalSpace { get; }

        Uri IconSource { get; }
        byte[] IconImage { get; }
    }

    [Flags]
    internal enum DriveUpdateItems : ushort
    {
        None = 0x0000,
        Name = 0x0001,
        Spaces = 0x0002,
        Icons = 0x0004,
        All = Name + Spaces + Icons,
    };

    internal class DriveItem : ObservableObject, IDriveItem
    {
        private readonly IStorageFolderReader storageFolderReader;

        public string Path { get; init; }

        private string name = string.Empty;
        public string Name
        {
            get => name;
            private set => SetProperty(ref name, value);
        }

        public DriveTypes DriveType { get; init; } = DriveTypes.Unknown;

        private ByteSize usedSpace = 0L;
        public ByteSize UsedSpace
        {
            get => usedSpace;
            private set => SetProperty(ref usedSpace, value);
        }

        private ByteSize freeSpace = 0L;
        public ByteSize FreeSpace
        {
            get => freeSpace;
            private set => SetProperty(ref freeSpace, value);
        }

        private ByteSize totalSpace = 0L;
        public ByteSize TotalSpace
        {
            get => totalSpace;
            private set => SetProperty(ref totalSpace, value);
        }

        public Uri IconSource { get; set; }
        public byte[] IconImage { get; set; }

        public DriveItem(StorageFolder root)
        {
            storageFolderReader = new StorageFolderReader(root);
        }

        public async Task UpdateAsync(DriveUpdateItems item)
        {
            if (item.HasFlag(DriveUpdateItems.Name))
            {
                await UpdateNameAsync();
            }
            if (item.HasFlag(DriveUpdateItems.Spaces))
            {
                await UpdateSpacesAsync();
            }
            if (item.HasFlag(DriveUpdateItems.Icons))
            {
                await UpdateIconsAsync();
            }
        }

        private async Task UpdateNameAsync()
            => Name = await storageFolderReader.GetPropertyAsync<string>("System.ItemNameDisplay") ?? string.Empty;

        public async Task UpdateSpacesAsync()
        {
            try
            {
                var properties = await storageFolderReader.GetPropertiesAsync<long>("System.Capacity", "System.Capacity");

                TotalSpace = properties["System.Capacity"];
                FreeSpace = properties["System.FreeSpace"];
                UsedSpace = FreeSpace <= TotalSpace ? TotalSpace - FreeSpace : 0L;
            }
            catch
            {
                UsedSpace = FreeSpace = TotalSpace = 0L;
            }
        }

        public async Task UpdateIconsAsync()
        {
            await Task.Yield();
        }
    }
}
