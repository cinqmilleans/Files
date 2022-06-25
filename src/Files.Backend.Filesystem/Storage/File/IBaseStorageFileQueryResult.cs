using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFileQueryResult : IStorageItemQueryResult
    {
        StorageFileQueryResult ToStorageFileQueryResult();

        IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync();
        IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems);
    }
}
