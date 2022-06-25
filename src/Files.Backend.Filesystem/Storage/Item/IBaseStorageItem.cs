using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBaseStorageItem : IStorageItem2, IStorageItemProperties2, IStorageItemPropertiesWithProvider
    {
        new IStorageItemExtraProperties Properties { get; }

        new IAsyncOperation<IBaseStorageFolder> GetParentAsync();
        new IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync();
    }
}
