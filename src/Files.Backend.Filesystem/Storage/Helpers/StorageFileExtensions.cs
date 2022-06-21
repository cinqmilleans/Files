using Files.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static async Task<List<IStorageItem>> ToStandardStorageItemsAsync(this IEnumerable<IStorageItem> items)
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
                catch (NotSupportedException)
                {
                    // Ignore items that can't be converted
                }
            }
            return newItems;
        }

        public static bool AreItemsInSameDrive(this IEnumerable<string> itemsPath, string destinationPath)
        {
            try
            {
                return itemsPath.Any(itemPath => Path.GetPathRoot(itemPath).Equals(Path.GetPathRoot(destinationPath), StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
        public static bool AreItemsInSameDrive(this IEnumerable<IStorageItem> storageItems, string destinationPath)
            => storageItems.Select(x => x.Path).AreItemsInSameDrive(destinationPath);
        public static bool AreItemsInSameDrive(this IEnumerable<IStorageItemWithPath> storageItems, string destinationPath)
            => storageItems.Select(x => x.Path).AreItemsInSameDrive(destinationPath);

        public static bool AreItemsAlreadyInFolder(this IEnumerable<string> itemsPath, string destinationPath)
        {
            try
            {
                return itemsPath.All(itemPath => Path.GetDirectoryName(itemPath).Equals(destinationPath.TrimPath(), StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
        public static bool AreItemsAlreadyInFolder(this IEnumerable<IStorageItem> storageItems, string destinationPath)
            => storageItems.Select(x => x.Path).AreItemsAlreadyInFolder(destinationPath);
        public static bool AreItemsAlreadyInFolder(this IEnumerable<IStorageItemWithPath> storageItems, string destinationPath)
            => storageItems.Select(x => x.Path).AreItemsAlreadyInFolder(destinationPath);

        public static BaseStorageFolder AsBaseStorageFolder(this IStorageItem item)
        {
            if (item == null)
            {
                return null;
            }
            else if (item.IsOfType(StorageItemTypes.Folder))
            {
                if (item is StorageFolder folder)
                {
                    return (BaseStorageFolder)folder;
                }
                else
                {
                    return item as BaseStorageFolder;
                }
            }
            return null;
        }
    }
}