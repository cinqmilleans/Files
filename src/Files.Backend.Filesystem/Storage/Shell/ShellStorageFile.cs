using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Shared;
using Files.Shared.Services;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using IO = System.IO;
using S = Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public class ShellStorageFile : BaseStorageFile
    {
        private static readonly IFullTrustAsker asker = Ioc.Default.GetService<IFullTrustAsker>();

        public override string Path { get; }
        public override string Name { get; }
        public override string DisplayName => Name;
        public override string ContentType => "application/octet-stream";
        public override string FileType => IO.Path.GetExtension(Name);
        public override string FolderRelativeId => $"0\\{Name}";

        public override string DisplayType { get; }

        public override DateTimeOffset DateCreated { get; }

        public override S.FileAttributes Attributes => S.FileAttributes.Normal | S.FileAttributes.ReadOnly;

        private IStorageItemExtraProperties properties;
        public override IStorageItemExtraProperties Properties => properties ??= new BaseBasicStorageItemExtraProperties(this);

        public ShellStorageFile(ShellFileItem item)
        {
            Name = item.FileName;
            Path = item.RecyclePath; // True path on disk
            DateCreated = item.CreatedDate;
            DisplayType = item.FileType;
        }

        public override IAsyncOperation<StorageFile> ToStorageFileAsync() => throw new NotSupportedException();

        public static ShellStorageFile FromShellItem(ShellFileItem item)
        {
            if (item is ShellLinkItem linkItem)
            {
                return new ShortcutStorageFile(linkItem);
            }
            else if (item.RecyclePath.Contains("$Recycle.Bin", StringComparison.Ordinal))
            {
                return new BinStorageFile(item);
            }
            else
            {
                return new ShellStorageFile(item);
            }
        }

        public static IAsyncOperation<BaseStorageFile> FromPathAsync(string path)
        {
            return AsyncInfo.Run<BaseStorageFile>(async (cancellationToken) =>
            {
                if (ShellStorageFolder.IsShellPath(path))
                {
                    if (await GetFile(path) is ShellFileItem file)
                    {
                        return FromShellItem(file);
                    }
                }
                return null;
            });
        }

        private static async Task<ShellFileItem> GetFile(string path)
        {
            if (asker is not null)
            {
                var parameter = new ValueSet
                {
                    ["Arguments"] = "ShellItem",
                    ["action"] = "Query",
                    ["item"] = path,
                };

                var response = await asker.GetResponseAsync(parameter);
                if (response.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<ShellFileItem>(response.Get("Item", string.Empty));
                }
            }
            return null;
        }

        public override bool IsEqual(IStorageItem item) => item?.Path == Path;
        public override bool IsOfType(StorageItemTypes type) => type is StorageItemTypes.File;

        public override IAsyncOperation<IBaseStorageFolder> GetParentAsync() => throw new NotSupportedException();
        public override IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync() => GetBasicProperties().AsAsyncOperation();

        public override IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace) => throw new NotSupportedException();
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder) => throw new NotSupportedException();
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName) => throw new NotSupportedException();
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option) => throw new NotSupportedException();

        public override IAsyncAction DeleteAsync() => throw new NotSupportedException();
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => throw new NotSupportedException();

        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (ShellStorageFolder.IsShellPath(Path))
                {
                    return null;
                }
                var zipFile = await StorageFile.GetFileFromPathAsync(Path);
                return await zipFile.GetThumbnailAsync(mode);
            });
        }
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (ShellStorageFolder.IsShellPath(Path))
                {
                    return null;
                }
                var zipFile = await StorageFile.GetFileFromPathAsync(Path);
                return await zipFile.GetThumbnailAsync(mode, requestedSize);
            });
        }
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (ShellStorageFolder.IsShellPath(Path))
                {
                    return null;
                }
                var zipFile = await StorageFile.GetFileFromPathAsync(Path);
                return await zipFile.GetThumbnailAsync(mode, requestedSize, options);
            });
        }

        public override IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace) => throw new NotSupportedException();
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder) => throw new NotSupportedException();
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName) => throw new NotSupportedException();
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option) => throw new NotSupportedException();

        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode) => throw new NotSupportedException();
        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode, StorageOpenOptions options) => throw new NotSupportedException();
        public override IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync() => throw new NotSupportedException();
        public override IAsyncOperation<IInputStream> OpenSequentialReadAsync() => throw new NotSupportedException();

        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync() => throw new NotSupportedException();
        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync(StorageOpenOptions options) => throw new NotSupportedException();

        public override IAsyncAction RenameAsync(string desiredName) => throw new NotSupportedException();
        public override IAsyncAction RenameAsync(string desiredName, NameCollisionOption option) => throw new NotSupportedException();

        private async Task<IBaseBasicProperties> GetBasicProperties()
        {
            if (await GetFile(Path) is ShellFileItem file)
            {
                return new ShellBasicProperties(file);
            }
            return new BaseBasicProperties();
        }
    }
}
