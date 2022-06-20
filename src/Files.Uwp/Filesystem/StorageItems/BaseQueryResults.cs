using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Uwp.Filesystem.StorageItems
{
    public class SystemStorageItemQueryResult : BaseStorageItemQueryResult
    {
        private StorageItemQueryResult StorageItemQueryResult { get; }

        public SystemStorageItemQueryResult(StorageItemQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageItemQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync(startIndex, maxNumberOfItems);
                return items.Select(x => x is StorageFolder ? (IStorageItem)new SystemStorageFolder(x as StorageFolder) : new SystemStorageFile(x as StorageFile)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync();
                return items.Select(x => x is StorageFolder ? (IStorageItem)new SystemStorageFolder(x as StorageFolder) : new SystemStorageFile(x as StorageFile)).ToList();
            });
        }

        public override StorageItemQueryResult ToStorageItemQueryResult() => StorageItemQueryResult;
    }

    public class SystemStorageFileQueryResult : BaseStorageFileQueryResult
    {
        private StorageFileQueryResult StorageFileQueryResult { get; }

        public SystemStorageFileQueryResult(StorageFileQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageFileQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = await StorageFileQueryResult.GetFilesAsync(startIndex, maxNumberOfItems);
                return items.Select(x => new SystemStorageFile(x)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = await StorageFileQueryResult.GetFilesAsync();
                return items.Select(x => new SystemStorageFile(x)).ToList();
            });
        }

        public override StorageFileQueryResult ToStorageFileQueryResult() => StorageFileQueryResult;
    }

    public class SystemStorageFolderQueryResult : BaseStorageFolderQueryResult
    {
        private StorageFolderQueryResult StorageFolderQueryResult { get; }

        public SystemStorageFolderQueryResult(StorageFolderQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageFolderQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = await StorageFolderQueryResult.GetFoldersAsync(startIndex, maxNumberOfItems);
                return items.Select(x => new SystemStorageFolder(x)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = await StorageFolderQueryResult.GetFoldersAsync();
                return items.Select(x => new SystemStorageFolder(x)).ToList();
            });
        }

        public override StorageFolderQueryResult ToStorageFolderQueryResult() => StorageFolderQueryResult;
    }
}
