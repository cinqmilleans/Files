using System;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class BaseStorageFileQueryResult : AbstractStorageItemQueryResult, IStorageItemQueryResult
    {
        public BaseStorageFileQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageFileQueryResult ToStorageFileQueryResult() => null;

        public virtual IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync()
            => GetFilteredItems(Folder.GetFilesAsync().AsTask());
        public virtual IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
            => GetFilteredItems(Folder.GetFilesAsync().AsTask(), startIndex, maxNumberOfItems);
    }
}
