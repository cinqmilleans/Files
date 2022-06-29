using Files.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class BaseStorageItemQueryResult : AbstractStorageItemQueryResult<IStorageItem>, IBaseStorageItemQueryResult
    {
        public BaseStorageItemQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageItemQueryResult ToStorageItemQueryResult() => null;

        public virtual IAsyncOperation<IEnumerable<IStorageItem>> GetItemsAsync()
            => ToResult(GetSourcesAsync());
        public virtual IAsyncOperation<IEnumerable<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfItems));

        private async Task<IEnumerable<IStorageItem>> GetSourcesAsync()
            => await Folder.GetItemsAsync();
        private async Task<IEnumerable<IStorageItem>> GetSourcesAsync(uint startIndex, uint maxNumberOfItems)
            => (await Folder.GetItemsAsync()).Limit(startIndex, maxNumberOfItems);
    }
}
