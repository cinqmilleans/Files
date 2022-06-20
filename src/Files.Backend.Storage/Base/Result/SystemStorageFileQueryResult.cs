using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class SystemStorageFileQueryResult : BaseStorageFileQueryResult
    {
        private StorageFileQueryResult StorageFileQueryResult { get; }

        public SystemStorageFileQueryResult(StorageFileQueryResult result) : base(result.Folder, result.GetCurrentQueryOptions())
            => StorageFileQueryResult = result;

        public override StorageFileQueryResult ToStorageFileQueryResult() => StorageFileQueryResult;

        public override IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync()
        {
            return AsyncInfo.Run<IImmutableList<IBaseStorageFile>>(async (cancellationToken) =>
            {
                var Files = await StorageFileQueryResult.GetFilesAsync();
                return Files.Select(file => new SystemStorageFile(file)).ToImmutableList();
            });
        }
        public override IAsyncOperation<IImmutableList<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfFiles)
        {
            return AsyncInfo.Run<IImmutableList<IBaseStorageFile>>(async (cancellationToken) =>
            {
                var Files = await StorageFileQueryResult.GetFilesAsync(startIndex, maxNumberOfFiles);
                return Files.Select(file => new SystemStorageFile(file)).ToImmutableList();
            });
        }
    }
}
