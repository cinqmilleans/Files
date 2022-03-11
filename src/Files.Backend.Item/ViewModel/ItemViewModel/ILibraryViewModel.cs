using System.Collections.ObjectModel;

namespace Files.Backend.Item
{
    public interface ILibraryViewModel
    {
        string DefaultFolderPath { get; }
        ReadOnlyObservableCollection<string> FolderPaths { get; }
    }
}
