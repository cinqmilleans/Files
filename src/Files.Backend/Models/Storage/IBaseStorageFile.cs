using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseStorageFile
        : IStorageItem2, IStorageFile, IStorageFile2, IStorageItemProperties2, IStorageItemPropertiesWithProvider, IStorageFilePropertiesWithAvailability
    {
        new IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        new IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        new IAsyncOperation<BaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);

        new IAsyncOperation<BaseBasicProperties> GetBasicPropertiesAsync();
        new IStorageItemExtraProperties Properties { get; }

        new IAsyncOperation<BaseStorageFolder> GetParentAsync();

        IAsyncOperation<StorageFile> ToStorageFileAsync();

        /*new IStorageItemExtraProperties Properties { get; }

        IAsyncOperation<IStorageFile> ToStorageFileAsync();

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();
        new IAsyncOperation<IBasicProperties> GetBasicPropertiesAsync();

        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);*/
    }
}
