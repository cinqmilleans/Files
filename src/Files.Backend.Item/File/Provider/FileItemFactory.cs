using CommunityToolkit.Mvvm.ComponentModel;
using Files.Backend.Item.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml.Media.Imaging;
using static Files.Backend.Item.Tools.NativeFindStorageItemHelper;
using IO = System.IO;

namespace Files.Backend.Item
{
    internal class FileItemFactory
    {
        private readonly CancellationToken cancellationToken;

        private static readonly IDisplayNameCache displayNameCache = DisplayNameCache.Instance;

        public FileItemFactory(CancellationToken cancellationToken)
            => this.cancellationToken = cancellationToken;

        public IFileItem Build(string path, WIN32_FIND_DATA data)
        {
            try
            {
                return GetFile(path, data);
            }
            catch (Exception ex)
            {
                throw new FileItemException(GetError(ex), "file item error", ex);
            }
        }

        private IFileItem GetFolder(string rootPath, WIN32_FIND_DATA data) => new FolderItem(rootPath, data);
        private IFileItem GetFile(string rootPath, WIN32_FIND_DATA data) => new FileItem(rootPath, data);

        private static DateTime Clean(DateTime date)
            => date < DateTime.FromFileTimeUtc(0) ? DateTime.MinValue : date;

        private static FileItemErrors GetError(Exception ex) => ex switch
        {
            UnauthorizedAccessException => FileItemErrors.Unauthorized,
            FileNotFoundException => FileItemErrors.NotFound,
            PathTooLongException => FileItemErrors.NameTooLong,
            _ => FileItemErrors.Unknown,
        };

        private class FolderItem : ObservableObject, IFileItem
        {
            public string Path { get; }
            public string Name { get; }
            public string Extension => string.Empty;

            public FileAttributes FileAttribute { get; }

            public ByteSize Size => ByteSize.Zero;

            public DateTime DateCreated { get; }
            public DateTime DateModified { get; }
            public DateTime DateAccessed { get; }

            public BitmapImage? MainImage => null;
            public BitmapImage? OverlayImage => null;

            public FolderItem(string rootPath, WIN32_FIND_DATA data)
            {
                Path = rootPath.CombineNameToPath(data.cFileName);
                Name = displayNameCache.ReadDisplayName(Path) ?? data.cFileName;
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute();
                DateCreated = data.ftCreationTime.ToDateTime();
                DateModified = data.ftLastWriteTime.ToDateTime();
                DateAccessed = data.ftLastAccessTime.ToDateTime();
            }
        }

        private class FileItem : ObservableObject, IFileItem
        {
            public string Path { get; }
            public string Name { get; }
            public string Extension { get; }

            public FileAttributes FileAttribute { get; }

            public ByteSize Size { get; }

            public DateTime DateCreated { get; }
            public DateTime DateModified { get; }
            public DateTime DateAccessed { get; }

            private BitmapImage? mainImage;
            public BitmapImage? MainImage
            {
                get => mainImage;
                internal set => SetProperty(ref mainImage, value);
            }

            private BitmapImage? overlayImage;
            public BitmapImage? OverlayImage
            {
                get => overlayImage;
                internal set => SetProperty(ref overlayImage, value);
            }

            public FileItem(string rootPath, WIN32_FIND_DATA data)
            {
                Path = rootPath.CombineNameToPath(data.cFileName);
                Name = displayNameCache.ReadDisplayName(Path) ?? data.cFileName;
                Extension = IO.Path.GetExtension(Path);
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute();
                Size = data.GetSize();
                DateCreated = data.ftCreationTime.ToDateTime();
                DateModified = data.ftLastWriteTime.ToDateTime();
                DateAccessed = data.ftLastAccessTime.ToDateTime();
            }
        }

        private class ShortcutItem : FileItem, IShortcutItem
        {
            public IShortcut Shortcut { get; } = new Shortcut();

            public ShortcutItem(string rootPath, WIN32_FIND_DATA data) : base(rootPath, data) {}
        }

        private class LibraryItem : FileItem, ILibraryItem
        {
            public ILibrary Library { get; } = new Library(Enumerable.Empty<string>());

            public LibraryItem(string rootPath, WIN32_FIND_DATA data) : base(rootPath, data) {}
        }

        private class RecycleBinItem : FileItem, IRecycleBinItem
        {
            public DateTime DateDeleted { get; }

            public RecycleBinItem(string rootPath, WIN32_FIND_DATA data) : base(rootPath, data) {}
        }

        private class FtpItem : FileItem, IFtpItem
        {
            public FtpItem(string rootPath, WIN32_FIND_DATA data) : base(rootPath, data) {}
        }

        private class Shortcut : IShortcut
        {
            public ShortcutTypes ShortcutType { get; init; } = ShortcutTypes.Unknown;

            public string TargetPath { get; init; } = string.Empty;
            public string Arguments { get; init; } = string.Empty;
            public string WorkingDirectory { get; init; } = string.Empty;
        }

        private class Library : ILibrary
        {
            public string DefaultFolderPath { get; init; } = string.Empty;

            public ImmutableArray<string> FolderPaths { get; }

            public Library(IEnumerable<string> folderPaths)
                => FolderPaths = ImmutableArray.Create(folderPaths.ToArray());
        }
    }
}
