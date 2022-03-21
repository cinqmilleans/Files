using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using IO = System.IO;
using static Files.Backend.Item.Tools.NativeFindStorageItemHelper;
using System.IO;
using Files.Backend.Item.Tools;

namespace Files.Backend.Item
{
    internal class FileItemFactory
    {
        public IFileItem Build(string path, WIN32_FIND_DATA data)
        {
            try
            {
                return BuildFileItem(path, data);
            }
            catch (Exception ex)
            {
                throw new FileItemException(GetError(ex), "file item error", ex);
            }
        }

        private IFileItem BuildFileItem(string path, WIN32_FIND_DATA data)
        {
            Item item = new Item
            {
                Path = path,
                Name = data.cFileName,
                FileAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToFileAttribute(),
                Size = data.GetSize(),
                DateCreated = data.ftCreationTime.ToDateTime(),
                DateModified = data.ftLastWriteTime.ToDateTime(),
                DateAccessed = data.ftLastAccessTime.ToDateTime(),
            };

            return new FileItem(item);
        }

        private static DateTime Clean(DateTime date)
            => date < DateTime.FromFileTimeUtc(0) ? DateTime.MinValue : date;

        private static FileItemErrors GetError(Exception ex) => ex switch
        {
            UnauthorizedAccessException => FileItemErrors.Unauthorized,
            FileNotFoundException => FileItemErrors.NotFound,
            PathTooLongException => FileItemErrors.NameTooLong,
            _ => FileItemErrors.Unknown,
        };

        private class Item
        {
            public string Path = string.Empty;
            public string Name = string.Empty;
            public string Extension = string.Empty;

            public FileAttributes FileAttribute = FileAttributes.None;

            public ByteSize Size = ByteSize.Zero;

            public DateTime DateCreated;
            public DateTime DateModified;
            public DateTime DateAccessed;
            public DateTime DateDeleted;

            public IShortcut? Shortcut;
            public ILibrary? Library;

            public BitmapImage? MainImage;
            public BitmapImage? OverlayImage;
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

            public FileItem(Item item)
            {
                Path = item.Path;
                Name = item.Name;
                Extension = item.Extension;
                FileAttribute = item.FileAttribute;
                Size = item.Size;
                DateCreated = item.DateCreated;
                DateModified = item.DateModified;
                DateAccessed = item.DateAccessed;
                MainImage = item.MainImage;
                OverlayImage = item.OverlayImage;
            }
        }

        private class ShortcutItem : FileItem, IShortcutItem
        {
            public IShortcut Shortcut { get; }

            public ShortcutItem(Item item) : base(item)
                => Shortcut = item.Shortcut ?? new Shortcut();
        }

        private class LibraryItem : FileItem, ILibraryItem
        {
            public ILibrary Library { get; }

            public LibraryItem(Item item) : base(item)
                => Library = item.Library ?? new Library(Enumerable.Empty<string>());
        }

        private class RecycleBinItem : FileItem, IRecycleBinItem
        {
            public DateTime DateDeleted { get; }

            public RecycleBinItem(Item item) : base(item)
                => DateDeleted = item.DateDeleted
        }

        private class FtpItem : FileItem, IFtpItem
        {
            public FtpItem(Item item) : base(item) {}
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
