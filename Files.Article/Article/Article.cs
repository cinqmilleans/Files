using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;
using IO = System.IO;
using static Files.Article.Helper.NativeFindStorageItemHelper;
using Files.Article.Extension;
using FluentFTP;

namespace Files.Article.Article
{
    public interface IArticle
    {
        string Path { get; }
    }

    public interface INamedArticle : IArticle
    {
        string Name { get; }
    }

    public interface ISizedArticle : IArticle
    {
        long Size { get; }
    }

    public interface IDatedArticle : IArticle
    {
        DateTime DateCreated { get; }
        DateTime DateModified { get; }
        DateTime DateAccessed { get; }
    }

    public interface IImagedArticle : IArticle
    {
        BitmapImage Icon { get; }
    }

    public interface IFileArticle : INamedArticle, ISizedArticle, IDatedArticle, IImagedArticle
    {
        FileTypes FileType { get; }
        FileAttributes FileAttribute { get; }
    }

    public interface IShortcutArticle : IFileArticle
    {
        ShortcutTypes ShortcutType { get; }

        string TargetPath { get; }
        string Arguments { get; }
        string WorkingDirectory { get; }
    }

    public interface ILibraryArticle : INamedArticle, IImagedArticle
    {
        public string DefaultFolder { get; }
        public IReadOnlyCollection<string> Folders { get; }
    }

    public enum FileTypes : ushort
    {
        Unknown,
        File,
        Folder,
    }

    public enum ShortcutTypes : ushort
    {
        Unknown,
        Executable,
        SymLink,
        Url,
    }

    [Flags]
    public enum FileAttributes : ushort
    {
        None = 0x0000,
        Archive = 0x0001,
        Compressed = 0x0002,
        Device = 0x0004,
        Directory = 0x0010,
        Encrypted = 0x0020,
        Hidden = 0x0040,
        Offline = 0x0100,
        ReadOnly = 0x0200,
        System = 0x0400,
        Temporary = 0x1000,
    }

    internal class FileArticle : IFileArticle
    {
        public string Path { get; }
        public string Name { get; }

        public FileTypes FileType { get; }
        public FileAttributes FileAttribute { get; }

        public long Size { get; }

        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }
        public DateTime DateAccessed { get; }

        public BitmapImage Icon { get; }

        public FileArticle(string path, WIN32_FIND_DATA data)
        {
            Path = path;
            Name = data.cFileName;
            FileType = IsDirectory(data) ? FileTypes.Folder : FileTypes.File;
            FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToAttribute();
            Size = data.GetSize();
            DateCreated = ToDateTime(ref data.ftCreationTime);
            DateModified = ToDateTime(ref data.ftLastWriteTime);
            DateAccessed = ToDateTime(ref data.ftLastAccessTime);
        }
    }

    internal class ShortcutArticle : FileArticle, IShortcutArticle
    {
        public ShortcutTypes ShortcutType { get; }

        public string TargetPath { get; }
        public string Arguments { get; }
        public string WorkingDirectory { get; }

        public ShortcutArticle(string path, WIN32_FIND_DATA data) : base(path, data)
        {
            var extension = IO.Path.GetExtension(TargetPath).ToLower();
            ShortcutType = extension switch
            {
                ".exe" => ShortcutTypes.Executable,
                ".lnk" => ShortcutTypes.SymLink,
                ".url" => ShortcutTypes.Url,
                _ => ShortcutTypes.Unknown,
            };
        }
    }

    public class FtpArticle : IFileArticle
    {
        public string Path { get; }
        public string Name { get; }

        public FileTypes FileType { get; }
        public FileAttributes FileAttribute { get; }

        public long Size { get; }

        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }
        public DateTime DateAccessed => DateModified;

        public BitmapImage Icon { get; }

        public FtpArticle(FtpListItem item, string folder)
        {
            var isFile = item.Type == FtpFileSystemObjectType.File;
            DateCreated = Clean(item.RawCreated);
            DateModified = Clean(item.RawModified);

            ItemNameRaw = item.Name;
            FileExtension = Path.GetExtension(item.Name);
            ItemPath = PathNormalization.Combine(folder, item.Name);
            PrimaryItemAttribute = isFile ? StorageItemTypes.File : StorageItemTypes.Folder;
            ItemPropertiesInitialized = false;

            var itemType = isFile ? "ItemTypeFile".GetLocalized() : "FileFolderListItem".GetLocalized();
            if (isFile && ItemName.Contains(".", StringComparison.Ordinal))
            {
                itemType = FileExtension.Trim('.') + " " + itemType;
            }

            ItemType = itemType;
            FileSizeBytes = item.Size;
            ContainsFilesOrFolders = !isFile;
            FileImage = null;
            FileSize = FileSizeBytes.ToSizeString();
            Opacity = 1;
            IsHiddenItem = false;
        }

        private static DateTime Clean(DateTime date)
            => date < DateTime.FromFileTimeUtc(0) ? DateTime.MinValue : date;
    }



    /*public class LibraryItem : ListedItem
    {
        public LibraryItem(LibraryLocationItem lib, string returnFormat = null) : base(null, returnFormat)
        {
            ItemPath = lib.Path;
            ItemNameRaw = lib.Text;
            PrimaryItemAttribute = StorageItemTypes.Folder;
            ItemType = "ItemTypeLibrary".GetLocalized();
            LoadCustomIcon = true;
            CustomIcon = lib.Icon;
            //CustomIconSource = lib.IconSource;
            LoadFileIcon = true;

            IsEmpty = lib.IsEmpty;
            DefaultSaveFolder = lib.DefaultSaveFolder;
            Folders = lib.Folders;
        }

        public bool IsEmpty { get; }

        public string DefaultSaveFolder { get; }

        public override string ItemName => ItemNameRaw;

        public ReadOnlyCollection<string> Folders { get; }

    }*/
}
