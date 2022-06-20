using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class BaseStorageFileQueryResult : AbstractStorageItemQueryResult<IBaseStorageFile>, IBaseStorageFileQueryResult
    {
        public BaseStorageFileQueryResult(IBaseStorageFolder folder, QueryOptions options) : base(folder, options) {}

        public virtual StorageFileQueryResult ToStorageFileQueryResult() => null;

        public virtual IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync()
            => GetItemsOperation();
        public virtual IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
            => GetItemsOperation(startIndex, maxNumberOfItems);

        protected override async Task<IEnumerable<IBaseStorageFile>> GetSourceItemsAsync() => await Folder.GetFilesAsync();
    }
}
