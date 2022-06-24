using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageItemQueryResult : IStorageItemQueryResult
    {
        StorageItemQueryResult ToStorageItemQueryResult();

        IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync();
        IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems);
    }
}