using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageItemQueryResult : BaseStorageItemQueryResult
    {
        private StorageItemQueryResult StorageItemQueryResult { get; }

        public SystemStorageItemQueryResult(StorageItemQueryResult result) : base(result.Folder, result.GetCurrentQueryOptions())
            => StorageItemQueryResult = result;

        public override StorageItemQueryResult ToStorageItemQueryResult() => StorageItemQueryResult;

        public override IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync()
        {
            return AsyncInfo.Run<IImmutableList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync();
                return items.Select(AsSystemStorageItem).ToImmutableList();
            });
        }
        public override IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IImmutableList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync(startIndex, maxNumberOfItems);
                return items.Select(AsSystemStorageItem).ToImmutableList();
            });
        }

        private static IStorageItem AsSystemStorageItem(IStorageItem item) => item switch
        {
            StorageFolder folder => new SystemStorageFolder(folder),
            StorageFile file => new SystemStorageFile(file),
            _ => throw new ArgumentException(nameof(item)),
        }
    }
}
