using System.Collections.Generic;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Article.Library
{
    public interface ILibrary
    {
        string Path { get; }
        string Name { get; }

        BitmapImage Icon { get; }

        string DefaultFolderPath { get; }
        IEnumerable<string> FolderPaths { get; }
    }

    internal class Library : ILibrary
    {
        public string Path { get; }
        public string Name { get; }

        public BitmapImage Icon { get; }

        public string DefaultFolderPath { get; }
        public IEnumerable<string> FolderPaths { get; }
    }
}
