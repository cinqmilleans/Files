using Files.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class BaseStorageFileQueryResult : AbstractStorageItemQueryResult<IBaseStorageFile>, IBaseStorageFileQueryResult
    {
        public BaseStorageFileQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageFileQueryResult ToStorageFileQueryResult() => null;

        public virtual IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync()
            => ToResult(GetSourcesAsync());
        public virtual IAsyncOperation<IReadOnlyList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfItems));

        private async Task<IEnumerable<IBaseStorageFile>> GetSourcesAsync()
            => await Folder.GetFilesAsync();
        private async Task<IEnumerable<IBaseStorageFile>> GetSourcesAsync(uint startIndex, uint maxNumberOfItems)
            => (await Folder.GetFilesAsync()).Limit(startIndex, maxNumberOfItems);
    }
}
