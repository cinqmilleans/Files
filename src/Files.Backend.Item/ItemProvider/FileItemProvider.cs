using System.Collections.Generic;
using System.Threading.Tasks;
using static Files.Backend.Item.Helper.NativeFindStorageItemHelper;
using static Files.Backend.Item.Helper.Win32FindDataExtension;
using IO = System.IO;

namespace Files.Backend.Item
{
    public interface IFileItemProvider : IItemProvider
    {
        new IAsyncEnumerable<IFileItem> ProvideItems();
    }

    public class FileItemProvider : IFileItemProvider
    {
        public string ParentPath { get; set; } = string.Empty;

        public System.Threading.CancellationToken CancellationToken { get; set; }

        public bool IncludeHiddens { get; set; } = false;
        public bool IncludeSystems { get; set; } = false;

        public bool ShowFolderSize { get; set; } = false;

        IAsyncEnumerable<IItem> IItemProvider.ProvideItems() => ProvideItems();
        public async IAsyncEnumerable<IFileItem> ProvideItems()
        {
            await Task.Delay(1);
            yield return null;
        }

        private IFileItem BuildFileItem(string path, WIN32_FIND_DATA data)
        {
            return new FileItem
            {
                Path = path,
                Name = data.cFileName,
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute(),
                Size = data.GetSize(),
                DateCreated = ToDateTime(ref data.ftCreationTime),
                DateModified = ToDateTime(ref data.ftLastWriteTime),
                DateAccessed = ToDateTime(ref data.ftLastAccessTime),
            };
        }
        private IFileItem BuildShortcutItem(string path, WIN32_FIND_DATA data)
        {
            return new ShortcutItem
            {
                Path = path,
                Name = data.cFileName,
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute(),
                Size = data.GetSize(),
                DateCreated = ToDateTime(ref data.ftCreationTime),
                DateModified = ToDateTime(ref data.ftLastWriteTime),
                DateAccessed = ToDateTime(ref data.ftLastAccessTime),
            };
        }
    }
}
