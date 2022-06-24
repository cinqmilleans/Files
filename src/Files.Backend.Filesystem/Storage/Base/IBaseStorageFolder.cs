using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFolder : IBaseStorageItem, IStorageFolder, IStorageFolder2, IStorageFolderQueryOperations
    {
        IAsyncOperation<StorageFolder> ToStorageFolderAsync();

        new IAsyncOperation<IStorageItem> GetItemAsync(string name);
        new IAsyncOperation<IImmutableList<IStorageItem>> GetItemsAsync();

        new IAsyncOperation<IBaseStorageFile> GetFileAsync(string name);
        new IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync();
        new IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        new IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);

        new IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name);
        new IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync();
        new IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);
        new IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);

        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);

        new IBaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);
        new IBaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        new IBaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);
    }
}
