using static Files.Article.Helper.NativeFindStorageItemHelper;

namespace Files.Article.Article
{
    public interface IShortcutArticle : IArticle
    {
        ShortcutTypes ShortcutType { get; }

        string TargetPath { get; }
        string Arguments { get; }
        string WorkingDirectory { get; }
    }

    public enum ShortcutTypes : ushort
    {
        Unknown,
        Executable,
        SymLink,
        Url,
    }

    internal class ShortcutArticle : Article, IShortcutArticle
    {
        public ShortcutTypes ShortcutType { get; }

        public string TargetPath { get; }
        public string Arguments { get; }
        public string WorkingDirectory { get; }

        public ShortcutArticle(string path, WIN32_FIND_DATA data) : base(path, data)
        {
            var extension = IO.Path.GetExtension(TargetPath).ToLower();
            ShortcutType = extension switch
            {
                ".exe" => ShortcutTypes.Executable,
                ".lnk" => ShortcutTypes.SymLink,
                ".url" => ShortcutTypes.Url,
                _ => ShortcutTypes.Unknown,
            };
        }
    }
}
