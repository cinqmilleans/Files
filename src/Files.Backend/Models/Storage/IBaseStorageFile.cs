using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseStorageFile : IStorageFile, IStorageFile2, IStorageItem2,
        IStorageItemProperties2, IStorageItemPropertiesWithProvider, IStorageFilePropertiesWithAvailability
    {
        new IStorageItemExtraProperties Properties { get; }

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();
        new IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync();

        IAsyncOperation<StorageFile> ToStorageFileAsync();

        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);
    }
}