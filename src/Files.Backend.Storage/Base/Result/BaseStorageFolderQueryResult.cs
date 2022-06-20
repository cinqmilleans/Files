using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class BaseStorageFolderQueryResult : AbstractStorageItemQueryResult<IBaseStorageFolder>, IBaseStorageFolderQueryResult
    {
        public BaseStorageFolderQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageFolderQueryResult ToStorageFolderQueryResult() => null;

        public virtual IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync()
            => ToResult(GetSourceItemsAsync());
        public virtual IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
            => ToResult(GetSourceItemsAsync(), startIndex, maxNumberOfItems);

        private async Task<IEnumerable<IBaseStorageFolder>> GetSourceItemsAsync() => await Folder.GetFoldersAsync();
    }
}
