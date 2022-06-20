using System;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class BaseStorageItemQueryResult : AbstractStorageItemQueryResult, IStorageItemQueryResult
    {
        public BaseStorageItemQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageItemQueryResult ToStorageItemQueryResult() => null;

        public virtual IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync()
            => GetFilteredItems(Folder.GetItemsAsync().AsTask());
        public virtual IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
            => GetFilteredItems(Folder.GetItemsAsync().AsTask(), startIndex, maxNumberOfItems);
    }
}