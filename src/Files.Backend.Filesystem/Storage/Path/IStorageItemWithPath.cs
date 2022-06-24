using Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public interface IStorageItemWithPath
    {
        public string Name { get; }
        public string Path { get; }

        public IStorageItem Item { get; }
        public StorageItemType ItemType { get; }
    }
}
