using Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public static class StorageFolderExtensions
    {
        public static BaseStorageFolder AsBaseStorageFolder(this IStorageItem item)
        {
            if (item is null || !item.IsOfType(StorageItemTypes.Folder))
            {
                return null;
            }
            return item is StorageFolder folder ? (BaseStorageFolder)folder : item as BaseStorageFolder;
        }
    }
}
