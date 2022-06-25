using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageItemQueryResult : IStorageItemQueryResult
    {
        StorageItemQueryResult ToStorageItemQueryResult();

        IAsyncOperation<IEnumerable<IStorageItem>> GetItemsAsync();
        IAsyncOperation<IEnumerable<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems);
    }
}