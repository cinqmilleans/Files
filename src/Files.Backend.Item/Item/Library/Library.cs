using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Backend.Item
{
    public class Library : ILibrary
    {
        public string DefaultFolderPath { get; init; } = string.Empty;

        public IReadOnlyCollection<string> FolderPaths { get; }

        public Library(IEnumerable<string> folderPaths) => FolderPaths = new ReadOnlyCollection<string>(folderPaths.ToList());

    }
}
