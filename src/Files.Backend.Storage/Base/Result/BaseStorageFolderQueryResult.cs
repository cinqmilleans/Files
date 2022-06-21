using Files.Shared.Extensions;
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
            => ToResult(GetSourcesAsync());
        public virtual IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfItems));

        private async Task<IEnumerable<IBaseStorageFolder>> GetSourcesAsync()
            => await Folder.GetFoldersAsync();
        private async Task<IEnumerable<IBaseStorageFolder>> GetSourcesAsync(uint startIndex, uint maxNumberOfItems)
            => (await Folder.GetFoldersAsync()).Limit(startIndex, maxNumberOfItems);
    }
}
