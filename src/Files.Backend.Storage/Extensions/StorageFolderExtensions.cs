using Windows.Storage;

namespace Files.Backend.Storage
{
    internal static class StorageFolderExtensions
    {
        public static BaseStorageFolder AsBaseStorageFolder(this IStorageItem item)
        {
            if (item is not null && item.IsOfType(StorageItemTypes.Folder))
            {
                return item is StorageFolder folder ? (BaseStorageFolder)folder : item as BaseStorageFolder;
            }
            return null;
        }
    }
}
