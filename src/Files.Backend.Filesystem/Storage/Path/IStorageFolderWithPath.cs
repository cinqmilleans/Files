namespace Files.Backend.Filesystem.Storage
{
    public interface IStorageFolderWithPath : IStorageItemWithPath
    {
        new IBaseStorageFolder Item { get; }
    }
}
