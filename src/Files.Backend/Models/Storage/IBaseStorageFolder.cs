using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    public interface IBaseStorageFolder :
        IStorageFolder, IStorageItem, IStorageFolderQueryOperations, IStorageItemProperties,
        IStorageItemProperties2, IStorageItem2, IStorageFolder2, IStorageItemPropertiesWithProvider
    {
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName);
        new IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options);
        new IAsyncOperation<IBaseStorageFile> GetFileAsync(string name);
        new IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name);
        new IAsyncOperation<IStorageItem> GetItemAsync(string name);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync();
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync();
        new IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync();

        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve);
        new IAsyncOperation<IReadOnlyList<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query);

        new BaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions);
        new BaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions);

        new IStorageItemExtraProperties Properties { get; }
        new IAsyncOperation<BaseBasicProperties> GetBasicPropertiesAsync();

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();

        IAsyncOperation<StorageFolder> ToStorageFolderAsync();
    }
}
