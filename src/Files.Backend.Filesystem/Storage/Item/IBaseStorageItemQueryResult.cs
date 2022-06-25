using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageItemQueryResult : IStorageItemQueryResult
    {
        StorageItemQueryResult ToStorageItemQueryResult();

        IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();
        IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems);
    }
}