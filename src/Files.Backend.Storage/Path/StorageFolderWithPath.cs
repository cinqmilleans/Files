using Files.Backend.Storage.Enums;
using Windows.Storage;
using IO = System.IO;

namespace Files.Backend.Storage
{
    public class StorageFolderWithPath : IStorageFolderWithPath
    {
        public string Path { get; }
        public string Name => Item?.Name ?? IO.Path.GetFileName(Path);

        IStorageItem IStorageItemWithPath.Item => Item;
        public IBaseStorageFolder Item { get; }

        public StorageItemType ItemType => StorageItemType.Directory;

        public StorageFolderWithPath(IBaseStorageFolder folder) : this(folder, folder.Path) {}
        public StorageFolderWithPath(IBaseStorageFolder folder, string path) => (Item, Path) = (folder, path);
    }
}
