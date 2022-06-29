using Files.Backend.Filesystem.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public static class StorageFileExtensions
    {
        public static BaseStorageFile AsBaseStorageFile(this IStorageItem item)
        {
            if (item is null || !item.IsOfType(StorageItemTypes.File))
            {
                return null;
            }
            return item is StorageFile file ? (BaseStorageFile)file : item as BaseStorageFile;
        }

        public async static Task<IList<StorageFileWithPath>> GetFilesWithPathAsync
            (this StorageFolderWithPath parentFolder, uint maxNumberOfItems = uint.MaxValue)
        => (await parentFolder.Item.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, maxNumberOfItems))
            .Select(x => new StorageFileWithPath(x, string.IsNullOrEmpty(x.Path) ? parentFolder.Path.CombinePath(x.Name) : x.Path)).ToList();

        public static async Task<IList<IStorageItem>> ToStandardStorageItemsAsync(this IEnumerable<IStorageItem> items)
        {
            var newItems = new List<IStorageItem>();
            foreach (var item in items)
            {
                try
                {
                    if (item is null)
                    {
                    }
                    else if (item.IsOfType(StorageItemTypes.File))
                    {
                        newItems.Add(await item.AsBaseStorageFile().ToStorageFileAsync());
                    }
                    else if (item.IsOfType(StorageItemTypes.Folder))
                    {
                        newItems.Add(await item.AsBaseStorageFolder().ToStorageFolderAsync());
                    }
                }
                catch (NotSupportedException) {} // Ignore items that can't be converted
            }
            return newItems;
        }
    }
}
