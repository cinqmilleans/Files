using System.Collections.ObjectModel;

namespace Files.Backend.Item
{
    public interface ILibrary
    {
        string DefaultFolderPath { get; }
        ReadOnlyObservableCollection<string> FolderPaths { get; }
    }
}
