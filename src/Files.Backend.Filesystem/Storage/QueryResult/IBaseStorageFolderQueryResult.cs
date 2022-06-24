using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFolderQueryResult : IStorageItemQueryResult
    {
        StorageFolderQueryResult ToStorageFolderQueryResult();

        IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync();
        IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems);
    }
}
