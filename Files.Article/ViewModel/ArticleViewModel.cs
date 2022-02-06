using Files.Article.Article;
using Files.Article.Library;
using System;
using IO = System.IO;

namespace Files.Article.ViewModel
{
    public interface IArticleViewModel
    {
        string Path { get; }
        string Name { get; }
        string Extension { get; }

        bool IsFile { get; }
        bool IsFolder { get; }

        ISizeViewModel Size { get; }

        IDateViewModel DateCreated { get; }
        IDateViewModel DateModified { get; }
        IDateViewModel DateAccessed { get; }

        IIconViewModel Icon { get; }

        bool IsShortCut { get; }
        IShortcutViewModel Shortcut { get; }

        bool IsLibrary { get; }
        ILibraryViewModel Library { get; }
    }

    internal class FileViewModel : IArticleViewModel
    {
        private readonly IArticle file;

        public string Path => file.Path;
        public string Name => file.Name;
        public string Extension => IO.Path.GetExtension(Name);

        public bool IsFile => true;
        public bool IsFolder => false;

        public ISizeViewModel Size { get; }

        public IDateViewModel DateCreated { get; }
        public IDateViewModel DateModified { get; }
        public IDateViewModel DateAccessed { get; }

        public IIconViewModel Icon { get; }

        public bool IsShortCut => false;
        public IShortcutViewModel Shortcut => null;

        public bool IsLibrary => false;
        public ILibraryViewModel Library => null;

        public FileViewModel(IArticle file)
        {
            if (file.ArticleType is not ArticleTypes.File)
            {
                throw new ArgumentException("The parameter file is not a file article.");
            }

            this.file = file;
            Size = new SizeViewModel(file.Size);
            DateCreated = new DateViewModel(file.DateCreated);
            DateModified = new DateViewModel(file.DateModified);
            DateAccessed = new DateViewModel(file.DateAccessed);
            Icon = new IconViewModel(file.Icon);
        }
    }

    internal class FolderViewModel : IArticleViewModel
    {
        private readonly IArticle folder;

        public string Path => folder.Path;
        public string Name => folder.Name;
        public string Extension => IO.Path.GetExtension(Name);

        public bool IsFile => false;
        public bool IsFolder => true;

        public ISizeViewModel Size { get; }

        public IDateViewModel DateCreated { get; }
        public IDateViewModel DateModified { get; }
        public IDateViewModel DateAccessed { get; }

        public IIconViewModel Icon { get; }

        public bool IsShortCut => false;
        public IShortcutViewModel Shortcut => null;

        public bool IsLibrary => false;
        public ILibraryViewModel Library => null;

        public FolderViewModel(IArticle folder)
        {
            if (folder.ArticleType is not ArticleTypes.Folder)
            {
                throw new ArgumentException("The parameter folder is not a folder article.");
            }

            this.folder = folder;
            Size = new SizeViewModel(folder.Size);
            DateCreated = new DateViewModel(folder.DateCreated);
            DateModified = new DateViewModel(folder.DateModified);
            DateAccessed = new DateViewModel(folder.DateAccessed);
            Icon = new IconViewModel(folder.Icon);
        }
    }

    internal class ShortcutArticleViewModel : IArticleViewModel
    {
        private readonly IArticle shortcut;

        public string Path => shortcut.Path;
        public string Name => shortcut.Name;
        public string Extension => IO.Path.GetExtension(Name);

        public bool IsFile => shortcut.ArticleType is ArticleTypes.File;
        public bool IsFolder => shortcut.ArticleType is ArticleTypes.Folder;

        public ISizeViewModel Size { get; }

        public IDateViewModel DateCreated { get; }
        public IDateViewModel DateModified { get; }
        public IDateViewModel DateAccessed { get; }

        public IIconViewModel Icon { get; }

        public bool IsShortCut => true;
        public IShortcutViewModel Shortcut { get; }

        public bool IsLibrary => false;
        public ILibraryViewModel Library => null;

        public ShortcutArticleViewModel(IShortcutArticle shortcut)
        {
            this.shortcut = shortcut;
            Size = new SizeViewModel(shortcut.Size);
            DateCreated = new DateViewModel(shortcut.DateCreated);
            DateModified = new DateViewModel(shortcut.DateModified);
            DateAccessed = new DateViewModel(shortcut.DateAccessed);
            Icon = new IconViewModel(shortcut.Icon);
            Shortcut = new ShortcutViewModel(shortcut);
        }
    }

    internal class LibraryArticleViewModel : IArticleViewModel
    {
        private readonly ILibrary library;

        public string Path => library.Path;
        public string Name => library.Name;
        public string Extension => string.Empty;

        public bool IsFile => false;
        public bool IsFolder => true;

        public ISizeViewModel Size => null;

        public IDateViewModel DateCreated => null;
        public IDateViewModel DateModified => null;
        public IDateViewModel DateAccessed => null;

        public IIconViewModel Icon { get; }

        public bool IsShortCut => false;
        public IShortcutViewModel Shortcut => null;

        public bool IsLibrary => true;
        public ILibraryViewModel Library { get; }

        public LibraryArticleViewModel(ILibrary library)
        {
            if (library is null)
            {
                throw new ArgumentNullException(nameof(library));
            }

            this.library = library;
            Icon = new IconViewModel(library.Icon);
            Library = new LibraryViewModel(library);
        }
    }
}
