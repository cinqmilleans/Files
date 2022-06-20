using System;
using System.Collections.Immutable;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class BaseStorageFolderQueryResult : AbstractStorageItemQueryResult, IStorageItemQueryResult
    {
        public BaseStorageFolderQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageFolderQueryResult ToStorageFolderQueryResult() => null;

        public virtual IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync()
            => GetFilteredItems(Folder.GetFoldersAsync().AsTask());
        public virtual IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
            => GetFilteredItems(Folder.GetFoldersAsync().AsTask(), startIndex, maxNumberOfItems);
    }
}
