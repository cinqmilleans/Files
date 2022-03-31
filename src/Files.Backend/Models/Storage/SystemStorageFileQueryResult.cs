using System;
using System.Collections.Generic;
using System.Text;

namespace Files.Backend.Models.Storage
{
    internal class SystemStorageFileQueryResult : BaseStorageFileQueryResult
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
