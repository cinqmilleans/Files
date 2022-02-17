using Files.BackEnd.Struct;
using Files.Common;
using Files.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Files.BackEnd
{
    public interface IDriveArticle : IArticle
    {
        DriveAttributes DriveAttribute { get; }

        ByteSize UsedSpace { get; }
        ByteSize FreeSpace { get; }
        ByteSize TotalSpace { get; }

        Uri IconSource { get; }
        byte[] IconImage { get; }
    }

    [Flags]
    public enum DriveAttributes : ushort
    {
        None = 0x0000,
        Fixed = 0x0001,
        Removable = 0x0002,
        Network = 0x0004,
        Ram = 0x0010,
        CDRom = 0x0020,
        FloppyDisk = 0x0040,
        Root = 0x0100,
        Virtual = 0x0200,
        Cloud = 0x0400,
    }

    internal class FixedDriveArticle : ObservableObject, IDriveArticle
    {
        public string Path { get; }
        public string Name { get; }

        public DriveAttributes DriveAttribute => DriveAttributes.Fixed;

        private ByteSize usedSpace = ByteSize.MinValue;
        public ByteSize UsedSpace
        {
            get => usedSpace;
            set => SetProperty(ref usedSpace, value);
        }

        private ByteSize freeSpace = ByteSize.MinValue;
        public ByteSize FreeSpace
        {
            get => freeSpace;
            set => SetProperty(ref freeSpace, value);
        }

        private ByteSize totalSpace = ByteSize.MinValue;
        public ByteSize TotalSpace
        {
            get => totalSpace;
            set => SetProperty(ref totalSpace, value);
        }

        public Uri IconSource { get; }

        private byte[] iconImage;
        public byte[] IconImage
        {
            get => iconImage;
            private set => SetProperty(ref iconImage, value);
        }

        /*public FixedDriveArticle()        public static async Task<DriveItem> CreateFromPropertiesAsync(StorageFolder root, string deviceId, DriveType type, IRandomAccessStream imageStream = null)
        {
            var item = new DriveItem();

            await CoreApplication.MainView.DispatcherQueue.EnqueueAsync(async () => await item.SetBitmapImage(imageStream));
            item.Text = root.DisplayName;
            item.Type = type;
            item.Path = string.IsNullOrEmpty(root.Path) ? $"\\\\?\\{root.Name}\\" : root.Path;
            item.DeviceID = deviceId;
            item.Root = root;
            _ = CoreApplication.MainView.DispatcherQueue.EnqueueAsync(() => item.UpdatePropertiesAsync());

            return item;
        }*/

        private async Task UpdateSpaces(StorageFolder root)
        {
            try
            {
                var properties = await root.Properties
                    .RetrievePropertiesAsync(new[] { "System.Capacity", "System.Capacity" })
                    .AsTask()
                    .WithTimeoutAsync(TimeSpan.FromSeconds(5));

                if (properties is null)
                {
                    UsedSpace = FreeSpace = TotalSpace = ByteSize.MinValue;
                }
                else
                {
                    TotalSpace = properties["System.Capacity"] is ulong totalSpace ? new(totalSpace) : ByteSize.MinValue;
                    FreeSpace = properties["System.FreeSpace"] is ulong freeSpace ? new(freeSpace) : ByteSize.MinValue;
                    UsedSpace = FreeSpace <= TotalSpace ? TotalSpace - FreeSpace : ByteSize.MinValue;
                }
            }
            catch
            {
                UsedSpace = FreeSpace = TotalSpace = ByteSize.MinValue;
            }
        }

        private async Task SetIconImage(IInputStream stream) => IconImage = await stream?.ToByteArrayAsync();
    }
}
