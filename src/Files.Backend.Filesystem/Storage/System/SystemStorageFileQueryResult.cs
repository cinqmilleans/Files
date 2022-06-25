using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageFileQueryResult : BaseStorageFileQueryResult
    {
        private StorageFileQueryResult StorageFileQueryResult { get; }

        public SystemStorageFileQueryResult(StorageFileQueryResult result) : base((BaseStorageFolder)result.Folder, result.GetCurrentQueryOptions())
            => StorageFileQueryResult = result;

        public override StorageFileQueryResult ToStorageFileQueryResult() => StorageFileQueryResult;

        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync()
            => ToResult(GetSourcesAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfFiles)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfFiles));

        public async Task<IEnumerable<IBaseStorageFile>> GetSourcesAsync()
        {
            var Files = await StorageFileQueryResult.GetFilesAsync();
            return Files.Select(file => new SystemStorageFile(file));
        }
        public async Task<IEnumerable<IBaseStorageFile>> GetSourcesAsync(uint startIndex, uint maxNumberOfFiles)
        {
            var Files = await StorageFileQueryResult.GetFilesAsync(startIndex, maxNumberOfFiles);
            return Files.Select(file => new SystemStorageFile(file));
        }
    }
}
