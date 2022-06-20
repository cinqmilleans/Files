using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public class SystemStorageFolderQueryResult : BaseStorageFolderQueryResult
    {
        private StorageFolderQueryResult StorageFolderQueryResult { get; }

        public SystemStorageFolderQueryResult(StorageFolderQueryResult result) : base(result.Folder, result.GetCurrentQueryOptions())
            => StorageFolderQueryResult = result;

        public override StorageFolderQueryResult ToStorageFolderQueryResult() => StorageFolderQueryResult;

        public override IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync()
        {
            return AsyncInfo.Run<IImmutableList<IBaseStorageFolder>>(async (cancellationToken) =>
            {
                var Folders = await StorageFolderQueryResult.GetFoldersAsync();
                return Folders.Select(folder => new SystemStorageFolder(folder)).ToImmutableList();
            });
        }
        public override IAsyncOperation<IImmutableList<IBaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfFolders)
        {
            return AsyncInfo.Run<IImmutableList<IBaseStorageFolder>>(async (cancellationToken) =>
            {
                var Folders = await StorageFolderQueryResult.GetFoldersAsync(startIndex, maxNumberOfFolders);
                return Folders.Select(folder => new SystemStorageFolder(folder)).ToImmutableList();
            });
        }
    }
}
