using Files.Backend.Storage.Enums;
using Windows.Storage;
using IO = System.IO;

namespace Files.Backend.Storage
{
    public class StorageFileWithPath : IStorageFileWithPath
    {
        public string Path { get; }
        public string Name => Item?.Name ?? IO.Path.GetFileName(Path);

        IStorageItem IStorageItemWithPath.Item => Item;
        public IBaseStorageFile Item { get; }

        public StorageItemType ItemType => StorageItemType.File;

        public StorageFileWithPath(IBaseStorageFile file) : this(file, file.Path) {}
        public StorageFileWithPath(IBaseStorageFile file, string path) => (Item, Path) = (file, path);
    }
}
