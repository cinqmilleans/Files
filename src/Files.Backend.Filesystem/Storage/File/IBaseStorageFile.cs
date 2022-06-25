using Windows.Foundation;
using Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageFile : IBaseStorageItem, IStorageFile, IStorageFile2, IStorageFilePropertiesWithAvailability
    {
        IAsyncOperation<StorageFile> ToStorageFileAsync();

        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName);
        new IAsyncOperation<IBaseStorageFile> CopyAsync(IStorageFolder destinationFolder, string desiredNewName, NameCollisionOption option);
    }
}
