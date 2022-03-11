using System.Collections.Generic;

namespace Files.Backend.Item
{
    public interface ILibrary
    {
        string DefaultFolderPath { get; }
        IReadOnlyCollection<string> FolderPaths { get; }
    }
}
