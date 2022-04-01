using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseStorageFolder
        : IStorageItem2, IStorageFolder, IStorageFolder2, IStorageItemProperties, IStorageItemPropertiesWithProvider, IStorageFolderQueryOperations
    {
        new IAsyncOperation<BaseStorageFile> CreateFileAsync(string desiredName);
        new IAsyncOperation<BaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<BaseStorageFolder> CreateFolderAsync(string desiredName);
        new IAsyncOperation<BaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<BaseStorageFile> GetFileAsync(string name);
        new IAsyncOperation<BaseStorageFolder> GetFolderAsync(string name);
        new IAsyncOperation<IStorageItem> GetItemAsync(string name);
        new IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync();
        new IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync();
        new IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();

        new IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        new IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);

        new BaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);

        new IAsyncOperation<BaseBasicProperties> GetBasicPropertiesAsync();
        new IStorageItemExtraProperties Properties { get; }

        new IAsyncOperation<BaseStorageFolder> GetParentAsync();

        IAsyncOperation<StorageFolder> ToStorageFolderAsync();



        /*new IStorageItemExtraProperties Properties { get; }

        IAsyncOperation<StorageFolder> ToStorageFolderAsync();

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();
        new IAsyncOperation<IBasicProperties> GetBasicPropertiesAsync();

        new IAsyncOperation<IStorageItem> GetItemAsync(string name);
        new IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();

        new IAsyncOperation<IBaseStorageFile> GetFileAsync(string name);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync();
        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);

        new IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync();
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);

        new IStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);
        new StorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        new StorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);*/
    }
}
