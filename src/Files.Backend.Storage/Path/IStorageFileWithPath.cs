namespace Files.Backend.Storage
{
    public interface IStorageFileWithPath : IStorageItemWithPath
    {
        new IBaseStorageFile Item { get; }
    }
}
