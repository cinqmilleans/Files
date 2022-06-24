namespace Files.Backend.Filesystem.Storage
{
    public interface IStorageFileWithPath : IStorageItemWithPath
    {
        new IBaseStorageFile Item { get; }
    }
}
