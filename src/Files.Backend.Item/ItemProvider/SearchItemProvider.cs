using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Files.Backend.Item.ItemProvider
{


    /*internal class SearchItemProvider : IFileItemProvider
    {
        public string Query { get; set; } = string.Empty;
        public string Folder { get; set; } = string.Empty;

        public uint ThumbnailSize { get; set; } = 24;
        public bool IncludeUnindexedItems { get; set; } = false;
        public bool IncludeUnindexedItems { get; set; } = false;
        public bool IncludeUnindexedItems { get; set; } = false;

        private async Task AddItemsAsyncForHome(IList<ListedItem> results, CancellationToken token)
        {
            foreach (var drive in App.DrivesManager.Drives.Where(x => !x.IsNetwork))
            {
                await AddItemsAsync(drive.Path, results, token);
            }
        }


        IAsyncEnumerable<IItem> IItemProvider.ProvideItems() => ProvideItems();
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

    }*/
}
