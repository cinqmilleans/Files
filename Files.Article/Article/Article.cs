using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Article.Article
{
    public enum ArticleTypes : ushort
    {
        Unknown,
        File,
        Folder,
    }

    public enum ShortcutTypes : ushort
    {
        Unknown,
        Executable,
        SymLink,
        Url,
    }

    public interface IArticle
    {
        string Path { get; }
        string Name { get; }

        ArticleTypes ArticleType { get; }

        long? Size { get; }

        DateTime? DateCreated { get; }
        DateTime? DateModified { get; }
        DateTime? DateAccessed { get; }

        BitmapImage IconDefault { get; }
        BitmapImage IconFile { get; }
        BitmapImage IconOverlay { get; }
    }

    public interface IShortcutArticle : IArticle
    {
        ShortcutTypes ShortcutType { get; }

        string TargetPath { get; }
        string Arguments { get; }
        string WorkingDirectory { get; }
    }
}
