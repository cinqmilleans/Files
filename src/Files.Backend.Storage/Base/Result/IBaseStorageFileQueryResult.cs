using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public interface IBaseStorageFileQueryResult : IStorageItemQueryResult
    {
        StorageFileQueryResult ToStorageFileQueryResult();

        IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync();
        IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems);
    }
}
