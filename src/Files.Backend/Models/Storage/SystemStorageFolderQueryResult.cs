using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    internal class SystemStorageFolderQueryResult : BaseStorageFolderQueryResult
    {
        private StorageFolderQueryResult StorageFolderQueryResult { get; }

        public SystemStorageFolderQueryResult(StorageFolderQueryResult result) : base(result.Folder, result.GetCurrentQueryOptions())
            => StorageFolderQueryResult = result;

        public override StorageFolderQueryResult ToStorageFolderQueryResult() => StorageFolderQueryResult;


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
    }
}
