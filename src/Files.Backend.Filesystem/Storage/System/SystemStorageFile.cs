using Files.Backend.Filesystem.Helpers;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using IO = System.IO;
using S = Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public sealed class SystemStorageFile : BaseStorageFile
    {
        public StorageFile File { get; }

        public override string Path => File?.Path;
        public override string Name => File?.Name;
        public override string DisplayName => File?.DisplayName;
        public override string ContentType => File.ContentType;
        public override string DisplayType => File?.DisplayType;
        public override string FileType => File.FileType;
        public override string FolderRelativeId => File?.FolderRelativeId;

        public override DateTimeOffset DateCreated => File.DateCreated;
        public override S.FileAttributes Attributes => File.Attributes;
        public override IStorageItemExtraProperties Properties => File?.Properties;

        public SystemStorageFile(StorageFile file) => File = file;

        public static IAsyncOperation<IBaseStorageFile> FromPathAsync(string path)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken)
                => new SystemStorageFile(await StorageFile.GetFileFromPathAsync(path)));

        public override IAsyncOperation<StorageFile> ToStorageFileAsync()
            => Task.FromResult(File).AsAsyncOperation();

        public override bool IsEqual(IStorageItem item) => File.IsEqual(item);
        public override bool IsOfType(StorageItemTypes type) => File.IsOfType(type);

        public override IAsyncOperation<IBaseStorageFolder> GetParentAsync()
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken)
                => new SystemStorageFolder(await File.GetParentAsync()));

        public override IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync()
            => AsyncInfo.Run<IBaseBasicProperties>(async (cancellationToken)
                => new SystemBasicProperties(await File.GetBasicPropertiesAsync()));

        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder)
            => CopyAsync(destinationFolder, Name, NameCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
            => CopyAsync(destinationFolder, desiredNewName, NameCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var destFolder = destinationFolder.AsBaseStorageFolder(); // Avoid calling IStorageFolder method
                try
                {
                    if (destFolder is SystemStorageFolder sysFolder)
                    {
                        // File created by CreateFileAsync will get immediately deleted on MTP?! (#7206)
                        return (BaseStorageFile)await File.CopyAsync(sysFolder.Folder, desiredNewName, option);
                    }
                    var destFile = await destFolder.CreateFileAsync(desiredNewName, option.Convert());
                    using (var inStream = await this.OpenStreamForReadAsync())
                    using (var outStream = await destFile.OpenStreamForWriteAsync())
                    {
                        await inStream.CopyToAsync(outStream);
                        await outStream.FlushAsync();
                    }
                    return destFile;
                }
                catch (UnauthorizedAccessException ex) // shortcuts & .url
                {
                    if (!string.IsNullOrEmpty(destFolder.Path))
                    {
                        var destination = IO.Path.Combine(destFolder.Path, desiredNewName);
                        var hFile = NativeHelpers.CreateFileForWrite(destination,
                            option == NameCollisionOption.ReplaceExisting);
                        if (!hFile.IsInvalid)
                        {
                            using (var inStream = await this.OpenStreamForReadAsync())
                            using (var outStream = new FileStream(hFile, FileAccess.Write))
                            {
                                await inStream.CopyToAsync(outStream);
                                await outStream.FlushAsync();
                            }
                            return new NativeStorageFile(destination, desiredNewName, DateTime.Now);
                        }
                    }
                    throw ex;
                }
            });
        }

        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
            => File.OpenAsync(accessMode);
        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode, StorageOpenOptions options)
            => File.OpenAsync(accessMode, options);

        public override IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync() => File.OpenReadAsync();
        public override IAsyncOperation<IInputStream> OpenSequentialReadAsync() => File.OpenSequentialReadAsync();

        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
            => File.OpenTransactedWriteAsync();
        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync(StorageOpenOptions options)
            => File.OpenTransactedWriteAsync(options);

        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder)
            => MoveAsync(destinationFolder, Name, NameCollisionOption.FailIfExists);
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
            => MoveAsync(destinationFolder, desiredNewName, NameCollisionOption.FailIfExists);
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var destFolder = destinationFolder.AsBaseStorageFolder(); // Avoid calling IStorageFolder method
                if (destFolder is SystemStorageFolder sysFolder)
                {
                    // File created by CreateFileAsync will get immediately deleted on MTP?! (#7206)
                    await File.MoveAsync(sysFolder.Folder, desiredNewName, option);
                    return;
                }
                await CopyAsync(destinationFolder, desiredNewName, option);
                // Move unsupported, copy but do not delete original
            });
        }

        public override IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var inStream = await this.OpenStreamForReadAsync();
                using var outStream = await fileToReplace.OpenStreamForWriteAsync();

                await inStream.CopyToAsync(outStream);
                await outStream.FlushAsync();
            });
        }
        public override IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var inStream = await this.OpenStreamForReadAsync();
                using var outStream = await fileToReplace.OpenStreamForWriteAsync();

                await inStream.CopyToAsync(outStream);
                await outStream.FlushAsync();
                // Move unsupported, copy but do not delete original
            });
        }

        public override IAsyncAction RenameAsync(string desiredName) => File.RenameAsync(desiredName);
        public override IAsyncAction RenameAsync(string desiredName, NameCollisionOption option) => File.RenameAsync(desiredName, option);

        public override IAsyncAction DeleteAsync() => File.DeleteAsync();
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => File.DeleteAsync(option);

        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode)
            => File.GetThumbnailAsync(mode);
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize)
            => File.GetThumbnailAsync(mode, requestedSize);
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
            => File.GetThumbnailAsync(mode, requestedSize, options);
    }
}
