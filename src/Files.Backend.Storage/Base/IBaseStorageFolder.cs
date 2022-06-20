using System.Collections.Generic;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public interface IBaseStorageFolder : IBaseStorageItem, IStorageFolder, IStorageFolder2, IStorageFolderQueryOperations
    {
        IAsyncOperation<StorageFolder> ToStorageFolderAsync();

        new IAsyncOperation<IStorageItem> GetItemAsync(string name);
        new IAsyncOperation<IEnumerable<IStorageItem>> GetItemsAsync();

        new IAsyncOperation<IBaseStorageFile> GetFileAsync(string name);
        new IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync();
        new IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        new IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);

        new IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name);
        new IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync();
        new IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);
        new IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);

        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);

        new BaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);
    }
}
