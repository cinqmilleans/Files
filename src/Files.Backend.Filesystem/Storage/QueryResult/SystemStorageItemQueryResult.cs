using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageItemQueryResult : BaseStorageFolderQueryResult
    {
        private StorageFolderQueryResult StorageFolderQueryResult { get; }

        public SystemStorageItemQueryResult(StorageFolderQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageFolderQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = await StorageFolderQueryResult.GetFoldersAsync(startIndex, maxNumberOfItems);
                return items.Select(x => new SystemStorageFolder(x)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = await StorageFolderQueryResult.GetFoldersAsync();
                return items.Select(x => new SystemStorageFolder(x)).ToList();
            });
        }

        public override StorageFolderQueryResult ToStorageFolderQueryResult() => StorageFolderQueryResult;
    }
}
