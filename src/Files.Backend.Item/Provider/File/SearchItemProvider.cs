using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.Backend.Item
{
    internal class SearchItemProvider : IFileItemProvider
    {
        private readonly IDriveManager driveManager = Ioc.Default.GetRequiredService<IDriveManager>();

        public string Query { get; init; } = string.Empty;
        public string Folder { get; init; } = string.Empty;

        public uint ThumbnailSize { get; init; } = 24;

        public bool IncludeHiddenItems { get; init; } = false;
        public bool IncludeSystemItems { get; init; } = false;
        public bool IncludeUnindexedItems { get; init; } = false;

        IAsyncEnumerable<IItem> IItemProvider.ProvideItems() => ProvideItems();
        public async IAsyncEnumerable<IFileItem> ProvideItems()
        {
            var items = ProvideItemsInLocalDrives();
            await foreach (var item in items)
            {
                yield return item;
            }
        }

        private async IAsyncEnumerable<IFileItem> ProvideItemsInLocalDrives()
        {
            var localDrives = driveManager.Drives.Where(drive => !drive.DriveType.HasFlag(DriveTypes.Network));
            foreach (var localDrive in localDrives)
            {
                var localItems = ProvideItemsInPath(localDrive.Path);
                await foreach (var localItem in localItems)
                {
                    yield return localItem;
                }
            }

        }
        private async IAsyncEnumerable<IFileItem> ProvideItemsInPath(string path)
        {
            await Task.Yield();
            yield return null;
        }


        /*IAsyncEnumerable<IItem> IItemProvider.ProvideItems() => ProvideItems();
        public IAsyncEnumerable<IFileItem> ProvideItems()
        {
            var workingFolder = await GetStorageFolderAsync(folder);

            var hiddenOnlyFromWin32 = false;
            if (workingFolder)
            {
                await SearchAsync(workingFolder, results, token);
                hiddenOnlyFromWin32 = (results.Count != 0);
            }

            if (!IsAQSQuery && (!hiddenOnlyFromWin32 || UserSettingsService.PreferencesSettingsService.AreHiddenItemsVisible))
            {
                await SearchWithWin32Async(folder, hiddenOnlyFromWin32, UsedMaxItemCount - (uint)results.Count, results, token);
            }
        }

        private static async Task<FilesystemResult<BaseStorageFolder>> GetStorageFolderAsync(string path)
            => await FilesystemTasks.Wrap(() => StorageFileExtensions.DangerousGetFolderFromPathAsync(path));

        private static async Task<FilesystemResult<BaseStorageFile>> GetStorageFileAsync(string path)
            => await FilesystemTasks.Wrap(() => StorageFileExtensions.DangerousGetFileFromPathAsync(path));
        */
    }
}
