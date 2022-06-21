using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageFolderQueryResult : BaseStorageItemQueryResult
    {
        private StorageItemQueryResult StorageItemQueryResult { get; }

        public SystemStorageFolderQueryResult(StorageItemQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
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
}
