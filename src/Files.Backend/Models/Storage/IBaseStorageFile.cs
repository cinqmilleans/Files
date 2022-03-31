using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseStorageFile
        : IStorageItem2, IStorageFile, IStorageFile2, IStorageItemProperties2, IStorageItemPropertiesWithProvider, IStorageFilePropertiesWithAvailability
    {
        new IStorageItemExtraProperties Properties { get; }

        IAsyncOperation<IStorageFile> ToStorageFileAsync();

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();
        new IAsyncOperation<IBasicProperties> GetBasicPropertiesAsync();

        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);
    }
}
