using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFolderQueryResult : IStorageItemQueryResult
    {
        StorageFolderQueryResult ToStorageFolderQueryResult();

        IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync();
        IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems);
    }
}
