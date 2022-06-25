using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
            => ToResult(GetSourcesAsync());
        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfItems));

        public async Task<IEnumerable<IStorageItem>> GetSourcesAsync()
        {
            var items = await StorageItemQueryResult.GetItemsAsync();
            return items.Select(AsSystemStorageItem).ToList();
        }
        public async Task<IEnumerable<IStorageItem>> GetSourcesAsync(uint startIndex, uint maxNumberOfItems)
        {
            var items = await StorageItemQueryResult.GetItemsAsync(startIndex, maxNumberOfItems);
            return items.Select(AsSystemStorageItem).ToList();
        }

        private static IStorageItem AsSystemStorageItem(IStorageItem item) => item switch
        {
            StorageFolder folder => new SystemStorageFolder(folder),
            StorageFile file => new SystemStorageFile(file),
            _ => throw new ArgumentException(nameof(item)),
        };
    }
}
