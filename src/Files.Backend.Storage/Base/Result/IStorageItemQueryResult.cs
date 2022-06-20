using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public interface IStorageItemQueryResult
    {
        IBaseStorageFolder Folder { get; }
        QueryOptions Options { get; }

    }
}