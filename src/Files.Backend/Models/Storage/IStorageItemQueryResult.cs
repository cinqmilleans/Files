using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage;

namespace Files.Backend.Models.Storage
{
    internal interface IStorageItemQueryResult
    {
        IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();
        IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems);
    }
}
