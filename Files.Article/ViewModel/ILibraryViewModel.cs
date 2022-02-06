using Files.Article.Library;
using System.Collections.Generic;
using System.Linq;

namespace Files.Article.ViewModel
{
    public interface ILibraryViewModel
    {
        bool IsEmpty { get; }
        string DefaultFolderPath { get; }
        IEnumerable<string> FolderPaths { get; }
    }

    internal class LibraryViewModel : ILibraryViewModel
    {
        private readonly ILibrary library;

        public bool IsEmpty => FolderPaths is null || !FolderPaths.Any();
        public string DefaultFolderPath => library.DefaultFolderPath;
        public IEnumerable<string> FolderPaths => library.FolderPaths;

        public LibraryViewModel(ILibrary library) => this.library = library;
    }
}
