using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFolderQueryResult : IStorageItemQueryResult
    {
        StorageFolderQueryResult ToStorageFolderQueryResult();

        IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync();
        IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems);
    }
}
