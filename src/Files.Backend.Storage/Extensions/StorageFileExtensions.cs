using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Backend.Storage.Extensions
{
    internal static class StorageFileExtensions
    {
        public async static Task<IBaseStorageFile> DangerousGetFileFromPathAsync
            (string value, IStorageFolderWithPath rootFolder = null, IStorageFolderWithPath parentFolder = null)
        => (await DangerousGetFileWithPathFromPathAsync(value, rootFolder, parentFolder)).Item;

        public async static Task<StorageFileWithPath> DangerousGetFileWithPathFromPathAsync
            (string value, StorageFolderWithPath rootFolder = null, StorageFolderWithPath parentFolder = null)
        {
            if (rootFolder is not null)
            {
                var currComponents = GetDirectoryPathComponents(value);

                if (parentFolder is not null && value.IsSubPathOf(parentFolder.Path))
                {
                    var folder = parentFolder.Item;
                    var prevComponents = GetDirectoryPathComponents(parentFolder.Path);
                    var path = parentFolder.Path;
                    foreach (var component in currComponents.ExceptBy(prevComponents, c => c.Path).SkipLast(1))
                    {
                        folder = await folder.GetFolderAsync(component.Title);
                        path = PathNormalization.Combine(path, folder.Name);
                    }
                    var file = await folder.GetFileAsync(currComponents.Last().Title);
                    path = PathNormalization.Combine(path, file.Name);
                    return new StorageFileWithPath(file, path);
                }
                else if (value.IsSubPathOf(rootFolder.Path))
                {
                    var folder = rootFolder.Item;
                    var path = rootFolder.Path;
                    foreach (var component in currComponents.Skip(1).SkipLast(1))
                    {
                        folder = await folder.GetFolderAsync(component.Title);
                        path = PathNormalization.Combine(path, folder.Name);
                    }
                    var file = await folder.GetFileAsync(currComponents.Last().Title);
                    path = PathNormalization.Combine(path, file.Name);
                    return new StorageFileWithPath(file, path);
                }
            }

            if (parentFolder is not null && !Path.IsPathRooted(value) && !ShellStorageFolder.IsShellPath(value)) // "::{" not a valid root
            {
                // Relative path
                var fullPath = Path.GetFullPath(Path.Combine(parentFolder.Path, value));
                return new StorageFileWithPath(await BaseStorageFile.GetFileFromPathAsync(fullPath));
            }
            return new StorageFileWithPath(await BaseStorageFile.GetFileFromPathAsync(value));
        }
        public async static Task<IList<StorageFileWithPath>> GetFilesWithPathAsync
            (this StorageFolderWithPath parentFolder, uint maxNumberOfItems = uint.MaxValue)
                => (await parentFolder.Item.GetFilesAsync(CommonFileQuery.DefaultQuery, 0, maxNumberOfItems))
                    .Select(x => new StorageFileWithPath(x, string.IsNullOrEmpty(x.Path) ? PathNormalization.Combine(parentFolder.Path, x.Name) : x.Path)).ToList();

    }
}
