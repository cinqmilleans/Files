using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class SystemStorageFolderQueryResult : BaseStorageFolderQueryResult
    {
        private StorageFolderQueryResult StorageFolderQueryResult { get; }

        public SystemStorageFolderQueryResult(StorageFolderQueryResult result) : base((BaseStorageFolder)result.Folder, result.GetCurrentQueryOptions())
            => StorageFolderQueryResult = result;

        public override StorageFolderQueryResult ToStorageFolderQueryResult() => StorageFolderQueryResult;

        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync()
            => ToResult(GetSourcesAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfFolders)
            => ToResult(GetSourcesAsync(startIndex, maxNumberOfFolders));

        private async Task<IEnumerable<IBaseStorageFolder>> GetSourcesAsync()
        {
            var folders = await StorageFolderQueryResult.GetFoldersAsync();
            return folders.Select(folder => new SystemStorageFolder(folder));
        }
        private async Task<IEnumerable<IBaseStorageFolder>> GetSourcesAsync(uint startIndex, uint maxNumberOfFolders)
        {
            var folders = await StorageFolderQueryResult.GetFoldersAsync(startIndex, maxNumberOfFolders);
            return folders.Select(folder => new SystemStorageFolder(folder));
        }
    }
}
