using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageFileQueryResult : BaseStorageFileQueryResult
    {
        private StorageFileQueryResult StorageFileQueryResult { get; }

        public SystemStorageFileQueryResult(StorageFileQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageFileQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = await StorageFileQueryResult.GetFilesAsync(startIndex, maxNumberOfItems);
                return items.Select(x => new SystemStorageFile(x)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = await StorageFileQueryResult.GetFilesAsync();
                return items.Select(x => new SystemStorageFile(x)).ToList();
            });
        }

        public override StorageFileQueryResult ToStorageFileQueryResult() => StorageFileQueryResult;
    }
}
