using Files.Backend.Filesystem.Helpers;
using Files.Shared.Extensions;
using FluentFTP;
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
    public sealed class FtpStorageFile : BaseStorageFile
    {
        public override string Path { get; }
        public override string Name { get; }
        public override string DisplayName => Name;
        public override string ContentType => "application/octet-stream";
        public override string FileType => IO.Path.GetExtension(Name);
        public string FtpPath { get; }
        public override string FolderRelativeId => $"0\\{Name}";

        public override string DisplayType
        {
            get
            {
                var itemType = "ItemTypeFile".ToLocalized();
                if (Name.Contains("."))
                {
                    itemType = IO.Path.GetExtension(Name).Trim('.') + " " + itemType;
                }
                return itemType;
            }
        }

        public override DateTimeOffset DateCreated { get; }
        public override S.FileAttributes Attributes { get; } = S.FileAttributes.Normal;
        public override IStorageItemExtraProperties Properties { get; }

        public FtpStorageFile(string path, string name, DateTimeOffset dateCreated) : base()
        {
            Path = path;
            Name = name;
            FtpPath = path.GetFtpPath();
            DateCreated = dateCreated;
        }
        public FtpStorageFile(string folder, FtpListItem ftpItem) : base()
        {
            Path = folder.CombinePath(ftpItem.Name);
            Name = ftpItem.Name;
            FtpPath = Path.GetFtpPath();
            DateCreated = ftpItem.RawCreated < DateTime.FromFileTimeUtc(0) ? DateTimeOffset.MinValue : ftpItem.RawCreated;
        }
        public FtpStorageFile(IStorageItemWithPath item) : base()
        {
            Path = item.Path;
            Name = IO.Path.GetFileName(item.Path);
            FtpPath = item.Path.GetFtpPath();
        }
        private FtpStorageFile() => Properties = new BaseBasicStorageItemExtraProperties(this);

        public static IAsyncOperation<IBaseStorageFile> FromPathAsync(string path)
            => path.IsFtpPath() && path.VerifyFtpPath()
                ? Task.FromResult<IBaseStorageFile>(new FtpStorageFile(new StorageFileWithPath(null, path))).AsAsyncOperation()
                : Task.FromResult<IBaseStorageFile>(null).AsAsyncOperation();

        public override IAsyncOperation<StorageFile> ToStorageFileAsync()
            => StorageFile.CreateStreamedFileAsync(Name, FtpDataStreamingHandler, null);

        public override bool IsEqual(IStorageItem item) => item?.Path == Path;
        public override bool IsOfType(StorageItemTypes type) => type is StorageItemTypes.File;

        public override IAsyncOperation<IBaseStorageFolder> GetParentAsync() => throw new NotSupportedException();

        public override IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync()
        {
            return AsyncInfo.Run<IBaseBasicProperties>(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return new BaseBasicProperties();
                }

                var item = await ftpClient.GetObjectInfoAsync(FtpPath);
                return item is null ? new BaseBasicProperties() : new FtpBasicProperties(item);
            });
        }

        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode)
        {
            return AsyncInfo.Run<IRandomAccessStream>(async (cancellationToken) =>
            {
                var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                if (accessMode is FileAccessMode.Read)
                {
                    var inStream = await ftpClient.OpenReadAsync(FtpPath, token: cancellationToken);
                    return new NonSeekableRandomAccessStreamForRead(inStream, (ulong)inStream.Length)
                    {
                        DisposeCallback = ftpClient.Dispose
                    };
                }
                return new NonSeekableRandomAccessStreamForWrite(await ftpClient.OpenWriteAsync(FtpPath, token: cancellationToken))
                {
                    DisposeCallback = ftpClient.Dispose
                };
            });
        }
        public override IAsyncOperation<IRandomAccessStream> OpenAsync(FileAccessMode accessMode, StorageOpenOptions options) => OpenAsync(accessMode);

        public override IAsyncOperation<IRandomAccessStreamWithContentType> OpenReadAsync()
        {
            return AsyncInfo.Run<IRandomAccessStreamWithContentType>(async (cancellationToken) =>
            {
                var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                var inStream = await ftpClient.OpenReadAsync(FtpPath, token: cancellationToken);
                var nsStream = new NonSeekableRandomAccessStreamForRead(inStream, (ulong)inStream.Length) { DisposeCallback = ftpClient.Dispose };
                return new StreamWithContentType(nsStream);
            });
        }
        public override IAsyncOperation<IInputStream> OpenSequentialReadAsync()
        {
            return AsyncInfo.Run<IInputStream>(async (cancellationToken) =>
            {
                var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                var inStream = await ftpClient.OpenReadAsync(FtpPath, token: cancellationToken);
                return new InputStreamWithDisposeCallback(inStream) { DisposeCallback = () => ftpClient.Dispose() };
            });
        }

        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync()
            => throw new NotSupportedException();
        public override IAsyncOperation<StorageStreamTransaction> OpenTransactedWriteAsync(StorageOpenOptions options)
            => throw new NotSupportedException();

        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder)
            => CopyAsync(destinationFolder, Name, NameCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName)
            => CopyAsync(destinationFolder, desiredNewName, NameCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                IBaseStorageFolder destFolder = destinationFolder.AsBaseStorageFolder();
                IBaseStorageFile file = await destFolder.CreateFileAsync(desiredNewName, option.Convert());

                var stream = await file.OpenStreamForWriteAsync();
                return await ftpClient.DownloadAsync(stream, FtpPath, token: cancellationToken) ? file : null;
            });
        }

        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder)
            => throw new NotSupportedException();
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName)
            => throw new NotSupportedException();
        public override IAsyncAction MoveAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option)
            => throw new NotSupportedException();

        public override IAsyncAction CopyAndReplaceAsync(IStorageFile fileToReplace) => throw new NotSupportedException();
        public override IAsyncAction MoveAndReplaceAsync(IStorageFile fileToReplace) => throw new NotSupportedException();

        public override IAsyncAction RenameAsync(string desiredName)
            => RenameAsync(desiredName, NameCollisionOption.FailIfExists);
        public override IAsyncAction RenameAsync(string desiredName, NameCollisionOption option)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return;
                }

                string destination = $"{FtpPath.GetParentPath()}/{desiredName}";
                var remoteExists = option is NameCollisionOption.ReplaceExisting ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
                bool isSuccessful = await ftpClient.MoveFileAsync(FtpPath, destination, remoteExists, cancellationToken);
                if (!isSuccessful && option is NameCollisionOption.GenerateUniqueName)
                {
                    // TODO: handle name generation
                }
            });
        }

        public override IAsyncAction DeleteAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (await ftpClient.EnsureConnectedAsync())
                {
                    await ftpClient.DeleteFileAsync(FtpPath, cancellationToken);
                }
            });
        }
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => DeleteAsync();

        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode)
            => Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize)
            => Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
            => Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();

        private FtpClient GetFtpClient()
        {
            string host = Path.GetFtpHost();
            ushort port = Path.GetFtpPort();
            var credentials = FtpManager.Credentials.Get(host, FtpManager.Anonymous);

            return new(host, port, credentials);
        }

        private async void FtpDataStreamingHandler(StreamedFileDataRequest request)
        {
            try
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    request.FailAndClose(StreamedFileFailureMode.CurrentlyUnavailable);
                    return;
                }

                using (var outStream = request.AsStreamForWrite())
                {
                    await ftpClient.DownloadAsync(outStream, FtpPath);
                    await outStream.FlushAsync();
                }
                request.Dispose();
            }
            catch
            {
                request.FailAndClose(StreamedFileFailureMode.Incomplete);
            }
        }
    }
}