using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public interface IStorageItemQueryResult
    {
        IBaseStorageFolder Folder { get; }
        QueryOptions Options { get; }
    }
}
