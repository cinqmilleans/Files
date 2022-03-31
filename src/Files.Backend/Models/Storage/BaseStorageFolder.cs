﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Files.Backend.Models.Storage
{
    internal abstract class BaseStorageFolder : IBaseStorageFolder
    {
        public static implicit operator BaseStorageFolder(StorageFolder value) => value != null ? new SystemStorageFolder(value) : null;

        public abstract IAsyncOperation<BaseStorageFile> CreateFileAsync(string desiredName);
        public abstract IAsyncOperation<BaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);
        public abstract IAsyncOperation<BaseStorageFolder> CreateFolderAsync(string desiredName);
        public abstract IAsyncOperation<BaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);
        public abstract IAsyncOperation<BaseStorageFile> GetFileAsync(string name);
        public abstract IAsyncOperation<BaseStorageFolder> GetFolderAsync(string name);
        public abstract IAsyncOperation<IStorageItem> GetItemAsync(string name);
        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync();
        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync();
        public abstract IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();
        public abstract IAsyncOperation<StorageFolder> ToStorageFolderAsync();
        public abstract IAsyncAction RenameAsync(string desiredName);
        public abstract IAsyncAction RenameAsync(string desiredName, NameCollisionOption option);
        public abstract IAsyncAction DeleteAsync();
        public abstract IAsyncAction DeleteAsync(StorageDeleteOption option);
        public abstract IAsyncOperation<BaseBasicProperties> GetBasicPropertiesAsync();
        public abstract bool IsOfType(StorageItemTypes type);

        public abstract FileAttributes Attributes { get; }
        public abstract DateTimeOffset DateCreated { get; }
        public abstract string Name { get; }
        public abstract string Path { get; }

        public abstract IAsyncOperation<IndexedState> GetIndexedStateAsync();
        public abstract StorageFileQueryResult CreateFileQuery();
        public abstract StorageFileQueryResult CreateFileQuery(CommonFileQuery query);
        public abstract BaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        public abstract StorageFolderQueryResult CreateFolderQuery();
        public abstract StorageFolderQueryResult CreateFolderQuery(CommonFolderQuery query);
        public abstract BaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);
        public abstract StorageItemQueryResult CreateItemQuery();
        public abstract BaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);
        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);
        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);

        public abstract IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);
        public abstract IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxItemsToRetrieve);
        public abstract bool AreQueryOptionsSupported(QueryOptions queryOptions);
        public abstract bool IsCommonFolderQuerySupported(CommonFolderQuery query);
        public abstract bool IsCommonFileQuerySupported(CommonFileQuery query);
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode);
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize);
        public abstract IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options);

        public abstract string DisplayName { get; }
        public abstract string DisplayType { get; }
        public abstract string FolderRelativeId { get; }
        public abstract IStorageItemExtraProperties Properties { get; }

        IAsyncOperation<StorageFile> IStorageFolder.CreateFileAsync(string desiredName)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CreateFileAsync(desiredName)).ToStorageFileAsync();
            });
        }

        IAsyncOperation<StorageFile> IStorageFolder.CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CreateFileAsync(desiredName, options)).ToStorageFileAsync();
            });
        }

        IAsyncOperation<StorageFolder> IStorageFolder.CreateFolderAsync(string desiredName)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CreateFolderAsync(desiredName)).ToStorageFolderAsync();
            });
        }

        IAsyncOperation<StorageFolder> IStorageFolder.CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await CreateFolderAsync(desiredName, options)).ToStorageFolderAsync();
            });
        }

        IAsyncOperation<StorageFile> IStorageFolder.GetFileAsync(string name)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await GetFileAsync(name)).ToStorageFileAsync();
            });
        }

        IAsyncOperation<StorageFolder> IStorageFolder.GetFolderAsync(string name)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await GetFolderAsync(name)).ToStorageFolderAsync();
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFile>> IStorageFolder.GetFilesAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFile>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFilesAsync()).Select(x => x.ToStorageFileAsync().AsTask()));
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFolder>> IStorageFolder.GetFoldersAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFolder>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFoldersAsync()).Select(x => x.ToStorageFolderAsync().AsTask()));
            });
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode, uint requestedSize)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public IAsyncOperation<StorageItemThumbnail> GetScaledImageAsThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
        {
            return Task.FromResult<StorageItemThumbnail>(null).AsAsyncOperation();
        }

        public abstract IAsyncOperation<BaseStorageFolder> GetParentAsync();
        public abstract bool IsEqual(IStorageItem item);
        public abstract IAsyncOperation<IStorageItem> TryGetItemAsync(string name);

        public StorageProvider Provider => null;

        IAsyncOperation<StorageFolder> IStorageItem2.GetParentAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                return await (await GetParentAsync()).ToStorageFolderAsync();
            });
        }

        public static IAsyncOperation<BaseStorageFolder> GetFolderFromPathAsync(string path)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                BaseStorageFolder folder = null;
                folder ??= await ZipStorageFolder.FromPathAsync(path);
                folder ??= await FtpStorageFolder.FromPathAsync(path);
                folder ??= await SystemStorageFolder.FromPathAsync(path);
                return folder;
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFile>> IStorageFolderQueryOperations.GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve)
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFile>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFilesAsync(query, startIndex, maxItemsToRetrieve)).Select(x => x.ToStorageFileAsync().AsTask()));
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFile>> IStorageFolderQueryOperations.GetFilesAsync(CommonFileQuery query)
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFile>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFilesAsync(query)).Select(x => x.ToStorageFileAsync().AsTask()));
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFolder>> IStorageFolderQueryOperations.GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve)
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFolder>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFoldersAsync(query, startIndex, maxItemsToRetrieve)).Select(x => x.ToStorageFolderAsync().AsTask()));
            });
        }

        IAsyncOperation<IReadOnlyList<StorageFolder>> IStorageFolderQueryOperations.GetFoldersAsync(CommonFolderQuery query)
        {
            return AsyncInfo.Run<IReadOnlyList<StorageFolder>>(async (cancellationToken) =>
            {
                return await Task.WhenAll((await GetFoldersAsync(query)).Select(x => x.ToStorageFolderAsync().AsTask()));
            });
        }

        StorageFileQueryResult IStorageFolderQueryOperations.CreateFileQueryWithOptions(QueryOptions queryOptions) => throw new NotSupportedException();
        StorageFolderQueryResult IStorageFolderQueryOperations.CreateFolderQueryWithOptions(QueryOptions queryOptions) => throw new NotSupportedException();
        StorageItemQueryResult IStorageFolderQueryOperations.CreateItemQueryWithOptions(QueryOptions queryOptions) => throw new NotSupportedException();

        IAsyncOperation<BasicProperties> IStorageItem.GetBasicPropertiesAsync()
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var item = await ToStorageFolderAsync();
                return await item.GetBasicPropertiesAsync();
            });
        }

        StorageItemContentProperties IStorageItemProperties.Properties
        {
            get
            {
                if (this is SystemStorageFolder sysFolder)
                {
                    return sysFolder.Folder.Properties;
                }
                return null;
            }
        }
    }

}
