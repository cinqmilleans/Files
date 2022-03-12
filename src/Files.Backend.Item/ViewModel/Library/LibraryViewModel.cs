using System.Collections.ObjectModel;

namespace Files.Backend.Item
{
    public class LibraryViewModel : ILibraryViewModel
    {
        private readonly ILibrary library;

        public string DefaultFolderPath => library.DefaultFolderPath;

        public ReadOnlyObservableCollection<string> FolderPaths { get; }

        public LibraryViewModel(ILibrary library)
        {
            this.library = library;
            FolderPaths = new(new());
        }
    }
}
