using Files.Backend.Filesystem.Helpers;
using Files.Shared.Extensions;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using IO = System.IO;
using S = Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public sealed class FtpStorageFolder : BaseStorageFolder
    {
        public override string Path { get; }
        public override string Name { get; }
        public override string DisplayName => Name;
        public override string DisplayType => "FileFolderListItem".ToLocalized();
        public string FtpPath { get; }
        public override string FolderRelativeId => $"0\\{Name}";

        public override DateTimeOffset DateCreated { get; }
        public override S.FileAttributes Attributes => S.FileAttributes.Directory;
        public override IStorageItemExtraProperties Properties { get; }

        public FtpStorageFolder(string path, string name, DateTimeOffset dateCreated) : this()
        {
            Path = path;
            Name = name;
            FtpPath = path.GetFtpPath();
            DateCreated = dateCreated;
        }
        public FtpStorageFolder(string folder, FtpListItem ftpItem) : this()
        {
            Path = folder.CombinePath(ftpItem.Name);
            Name = ftpItem.Name;
            FtpPath = Path.GetFtpPath();
            DateCreated = ftpItem.RawCreated < DateTime.FromFileTimeUtc(0) ? DateTimeOffset.MinValue : ftpItem.RawCreated;
        }
        public FtpStorageFolder(IStorageItemWithPath item) : this()
        {
            Path = item.Path;
            Name = IO.Path.GetFileName(item.Path);
            FtpPath = item.Path.GetFtpPath();
        }
        private FtpStorageFolder() => Properties = new BaseBasicStorageItemExtraProperties(this);

        public static IAsyncOperation<IBaseStorageFolder> FromPathAsync(string path)
            => path.IsFtpPath() && path.VerifyFtpPath()
                ? Task.FromResult<IBaseStorageFolder>(new FtpStorageFolder(new StorageFolderWithPath(null, path))).AsAsyncOperation()
                : Task.FromResult<IBaseStorageFolder>(null).AsAsyncOperation();

        public override IAsyncOperation<StorageFolder> ToStorageFolderAsync() => throw new NotSupportedException();

        public FtpStorageFolder CloneWithPath(string path) => new(new StorageFolderWithPath(null, path));

        public override bool IsEqual(IStorageItem item) => item?.Path == Path;
        public override bool IsOfType(StorageItemTypes type) => type is StorageItemTypes.Folder;

        public override IAsyncOperation<IndexedState> GetIndexedStateAsync() => Task.FromResult(IndexedState.NotIndexed).AsAsyncOperation();

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

        public override IAsyncOperation<IStorageItem> GetItemAsync(string name)
        {
            return AsyncInfo.Run<IStorageItem>(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                var item = await ftpClient.GetObjectInfoAsync(Path.CombinePath(name).GetFtpPath());
                if (item is not null)
                {
                    if (item.Type is FtpFileSystemObjectType.File)
                    {
                        return new FtpStorageFile(Path, item);
                    }
                    if (item.Type is FtpFileSystemObjectType.Directory)
                    {
                        return new FtpStorageFolder(Path, item);
                    }
                }
                return null;
            });
        }
        public override IAsyncOperation<IStorageItem> TryGetItemAsync(string name)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                try
                {
                    return await GetItemAsync(name);
                }
                catch
                {
                    return null;
                }
            });
        }
        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                var items = new List<IStorageItem>();
                var list = await ftpClient.GetListingAsync(FtpPath);
                foreach (var item in list)
                {
                    if (item.Type is FtpFileSystemObjectType.File)
                    {
                        items.Add(new FtpStorageFile(Path, item));
                    }
                    else if (item.Type is FtpFileSystemObjectType.Directory)
                    {
                        items.Add(new FtpStorageFolder(Path, item));
                    }
                }
                return (IReadOnlyList<IStorageItem>)items;
            });
        }
        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken)
                => (await GetItemsAsync()).Skip((int)startIndex).Take((int)maxItemsToRetrieve).ToList());

        public override IAsyncOperation<IBaseStorageFile> GetFileAsync(string name)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) => await GetItemAsync(name) as BaseStorageFile);
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync()
            => AsyncInfo.Run<IEnumerable<IBaseStorageFile>>(async (cancellationToken)
                => (await GetItemsAsync())?.OfType<FtpStorageFile>());
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query)
            => AsyncInfo.Run(async (cancellationToken) => await GetFilesAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run(async (cancellationToken)
                => (await GetFilesAsync()).Skip((int)startIndex).Take((int)maxItemsToRetrieve));

        public override IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name)
            => AsyncInfo.Run(async (cancellationToken) => await GetItemAsync(name) as IBaseStorageFolder);
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync()
            => AsyncInfo.Run<IEnumerable<IBaseStorageFolder>>(async (cancellationToken)
                => (await GetItemsAsync())?.OfType<FtpStorageFolder>());
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query)
            => AsyncInfo.Run(async (cancellationToken) => await GetFoldersAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync
                (CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run(async (cancellationToken)
                => (await GetFoldersAsync()).Skip((int)startIndex).Take((int)maxItemsToRetrieve));

        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName)
            => CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            return AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    return null;
                }

                using var stream = new MemoryStream();

                string remotePath = $"{FtpPath}/{desiredName}";
                var ftpRemoteExists = options is CreationCollisionOption.ReplaceExisting ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;

                var result = await ftpClient.UploadAsync(stream, remotePath, ftpRemoteExists);
                if (result is FtpStatus.Success)
                {
                    return new FtpStorageFile(new StorageFileWithPath(null, $"{Path}/{desiredName}"));
                }
                if (result is FtpStatus.Skipped)
                {
                    if (options is CreationCollisionOption.FailIfExists)
                    {
                        throw new IOException("File already exists.");
                    }
                    return null;
                }

                throw new IOException($"Failed to create file {remotePath}.");
            });
        }

        public override IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName)
            => CreateFolderAsync(desiredName, CreationCollisionOption.FailIfExists);
        public override IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            return AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) =>
            {
                using var ftpClient = GetFtpClient();
                if (!await ftpClient.EnsureConnectedAsync())
                {
                    throw new IOException($"Failed to connect to FTP server.");
                }

                string fileName = $"{FtpPath}/{desiredName}";
                if (ftpClient.DirectoryExists(fileName))
                {
                    return new FtpStorageFolder(new StorageFileWithPath(null, fileName));
                }

                bool replaceExisting = options is CreationCollisionOption.ReplaceExisting;
                bool isSuccessful = await ftpClient.CreateDirectoryAsync(fileName, replaceExisting, cancellationToken);
                if (!isSuccessful)
                {
                    throw new IOException($"Failed to create folder {desiredName}.");
                }

                return new FtpStorageFolder(new StorageFileWithPath(null, $"{Path}/{desiredName}"));
            });
        }

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

                string destination = $"{FtpPath.GetParentDir()}/{desiredName}";
                var ftpOption = option is NameCollisionOption.ReplaceExisting ? FtpRemoteExists.Overwrite : FtpRemoteExists.Skip;
                bool isSuccessful = await ftpClient.MoveDirectoryAsync(FtpPath, destination, ftpOption, cancellationToken);
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
                    await ftpClient.DeleteDirectoryAsync(FtpPath, cancellationToken);
                }
            });
        }
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => DeleteAsync();

        public override bool AreQueryOptionsSupported(QueryOptions queryOptions) => false;
        public override bool IsCommonFileQuerySupported(CommonFileQuery query) => false;
        public override bool IsCommonFolderQuerySupported(CommonFolderQuery query) => false;

        public override StorageItemQueryResult CreateItemQuery() => throw new NotSupportedException();
        public override IBaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions)
            => new BaseStorageItemQueryResult(this, queryOptions);

        public override StorageFileQueryResult CreateFileQuery() => throw new NotSupportedException();
        public override StorageFileQueryResult CreateFileQuery(CommonFileQuery query) => throw new NotSupportedException();
        public override IBaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions)
            => new BaseStorageFileQueryResult(this, queryOptions);

        public override StorageFolderQueryResult CreateFolderQuery() => throw new NotSupportedException();
        public override StorageFolderQueryResult CreateFolderQuery(CommonFolderQuery query) => throw new NotSupportedException();
        public override IBaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions)
            => new BaseStorageFolderQueryResult(this, queryOptions);

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
    }
}
