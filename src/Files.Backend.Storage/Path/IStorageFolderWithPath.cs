namespace Files.Backend.Storage
{
    public interface IStorageFolderWithPath : IStorageItemWithPath
    {
        new IBaseStorageFolder Item { get; }
    }
}
