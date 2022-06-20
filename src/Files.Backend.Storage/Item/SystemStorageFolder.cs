﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class SystemStorageFolder : BaseStorageFolder
    {
        public StorageFolder Folder { get; }

        public override string Path => Folder.Path;
        public override string Name => Folder.Name;
        public override string DisplayName => Folder.DisplayName;
        public override string DisplayType => Folder.DisplayType;
        public override string FolderRelativeId => Folder.FolderRelativeId;

        public override DateTimeOffset DateCreated => Folder.DateCreated;
        public override FileAttributes Attributes => Folder.Attributes;
        public override IStorageItemExtraProperties Properties => Folder.Properties;

        public SystemStorageFolder(StorageFolder folder) => Folder = folder;

        public static IAsyncOperation<IBaseStorageFolder> FromPathAsync(string path)
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => new SystemStorageFolder(await StorageFolder.GetFolderFromPathAsync(path)));

        public override IAsyncOperation<StorageFolder> ToStorageFolderAsync() => Task.FromResult(Folder).AsAsyncOperation();

        public override bool IsEqual(IStorageItem item) => Folder.IsEqual(item);
        public override bool IsOfType(StorageItemTypes type) => Folder.IsOfType(type);

        public override IAsyncOperation<IBaseStorageFolder> GetParentAsync()
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => new SystemStorageFolder(await Folder.GetParentAsync()));
        public override IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync()
            => AsyncInfo.Run<IBaseBasicProperties>(async (cancellationToken) => new SystemFolderBasicProperties(await Folder.GetBasicPropertiesAsync()));

        public IAsyncOperation<IndexedState> GetIndexedStateAsync() => Folder.GetIndexedStateAsync();

        public override IAsyncOperation<IStorageItem> GetItemAsync(string name)
            => Folder.GetItemAsync(name);
        public override IAsyncOperation<IStorageItem> TryGetItemAsync(string name)
            => Folder.TryGetItemAsync(name);
        public override IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync()
            => Folder.GetItemsAsync();
        public override IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxItemsToRetrieve)
            => Folder.GetItemsAsync(startIndex, maxItemsToRetrieve);

        public override IAsyncOperation<IBaseStorageFile> GetFileAsync(string name)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) => new SystemStorageFile(await Folder.GetFileAsync(name)));
        public override IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync()
            => AsyncInfo.Run<IImmutableList<IBaseStorageFile>>(async (cancellationToken)
                => (await Folder.GetFilesAsync()).Select(item => new SystemStorageFile(item)).ToList()
            );
        public override IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query)
            => AsyncInfo.Run<IImmutableList<IBaseStorageFile>>(async (cancellationToken)
                => (await Folder.GetFilesAsync(query)).Select(x => new SystemStorageFile(x)).ToImmutableList<IBaseStorageFolder>());
        public override IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run<IImmutableList<IBaseStorageFile>>(async (cancellationToken)
                => (await Folder.GetFilesAsync(query, startIndex, maxItemsToRetrieve)).Select(x => new SystemStorageFile(x)).ToImmutableList<IBaseStorageFolder>());

        public override IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name)
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => new SystemStorageFolder(await Folder.GetFolderAsync(name)));
        public override IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync()
            => AsyncInfo.Run<IImmutableList<IBaseStorageFolder>>(async (cancellationToken)
                => (await Folder.GetFoldersAsync()).Select(item => new SystemStorageFolder(item)).ToImmutableList<IBaseStorageFolder>()
            );
        public override IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query)
            => AsyncInfo.Run<IImmutableList<IBaseStorageFolder>>(async (cancellationToken)
                => (await Folder.GetFoldersAsync(query)).Select(x => new SystemStorageFolder(x)).ToImmutableList());
        public override IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run<IImmutableList<IBaseStorageFolder>>(async (cancellationToken)
                => (await Folder.GetFoldersAsync(query, startIndex, maxItemsToRetrieve)).Select(x => new SystemStorageFolder(x)).ToImmutableList<IBaseStorageFolder>());

        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) => new SystemStorageFile(await Folder.CreateFileAsync(desiredName)));
        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) => new SystemStorageFile(await Folder.CreateFileAsync(desiredName, options)));

        public IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName)
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => new SystemStorageFolder(await Folder.CreateFolderAsync(desiredName)));
        public IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => new SystemStorageFolder(await Folder.CreateFolderAsync(desiredName, options)));

        public override IAsyncAction RenameAsync(string desiredName) => Folder.RenameAsync(desiredName);
        public override IAsyncAction RenameAsync(string desiredName, NameCollisionOption option) => Folder.RenameAsync(desiredName, option);

        public override IAsyncAction DeleteAsync() => Folder.DeleteAsync();
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => Folder.DeleteAsync(option);

        public override bool AreQueryOptionsSupported(QueryOptions queryOptions) => Folder.AreQueryOptionsSupported(queryOptions);
        public override bool IsCommonFileQuerySupported(CommonFileQuery query) => Folder.IsCommonFileQuerySupported(query);
        public override bool IsCommonFolderQuerySupported(CommonFolderQuery query) => Folder.IsCommonFolderQuerySupported(query);

        public override StorageItemQueryResult CreateItemQuery()
            => Folder.CreateItemQuery();
        publicoverride BaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions)
            => new SystemStorageItemQueryResult(Folder.CreateItemQueryWithOptions(queryOptions));

        public override StorageFileQueryResult CreateFileQuery()
            => Folder.CreateFileQuery();
        public override StorageFileQueryResult CreateFileQuery(CommonFileQuery query)
            => Folder.CreateFileQuery(query);
        public override BaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions)
            => new SystemStorageFileQueryResult(Folder.CreateFileQueryWithOptions(queryOptions));

        public override StorageFolderQueryResult CreateFolderQuery()
            => Folder.CreateFolderQuery();
        public override StorageFolderQueryResult CreateFolderQuery(CommonFolderQuery query)
            => Folder.CreateFolderQuery(query);
        public override IBaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions)
            => new SystemStorageFolderQueryResult(Folder.CreateFolderQueryWithOptions(queryOptions));

        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode)
            => Folder.GetThumbnailAsync(mode);
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize)
            => Folder.GetThumbnailAsync(mode, requestedSize);
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
            => Folder.GetThumbnailAsync(mode, requestedSize, options);

        private class SystemFolderBasicProperties : IBaseBasicProperties
        {
            private readonly IStorageItemExtraProperties basicProps;

            public ulong Size => (basicProps as BasicProperties)?.Size ?? 0;

            public DateTimeOffset ItemDate => (basicProps as BasicProperties)?.ItemDate ?? DateTimeOffset.Now;
            public DateTimeOffset DateModified => (basicProps as BasicProperties)?.DateModified ?? DateTimeOffset.Now;

            public SystemFolderBasicProperties(IStorageItemExtraProperties basicProps) => this.basicProps = basicProps;

            public IAsyncOperation<IDictionary<string, object>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
                => basicProps.RetrievePropertiesAsync(propertiesToRetrieve);

            public IAsyncAction SavePropertiesAsync()
                => basicProps.SavePropertiesAsync();
            public IAsyncAction SavePropertiesAsync([HasVariant] IEnumerable<KeyValuePair<string, object>> propertiesToSave)
                => basicProps.SavePropertiesAsync(propertiesToSave);
        }
    }
}