using Files.Article.Extension;
using FluentFTP;
using System;
using Windows.UI.Xaml.Media.Imaging;
using static Files.Article.Helper.NativeFindStorageItemHelper;
using IO = System.IO;

namespace Files.Article.Article
{
    public interface IArticle
    {
        string Path { get; }
        string Name { get; }

        ArticleTypes ArticleType { get; }
        ArticleAttributes ArticleAttribute { get; }

        long Size { get; }

        DateTime DateCreated { get; }
        DateTime DateModified { get; }
        DateTime DateAccessed { get; }

        BitmapImage Icon { get; }
    }

    public enum ArticleTypes : ushort
    {
        Unknown,
        File,
        Folder,
    }

    [Flags]
    public enum ArticleAttributes : ushort
    {
        None = 0x0000,
        Archive = 0x0001,
        Compressed = 0x0002,
        Device = 0x0004,
        Directory = 0x0010,
        Encrypted = 0x0020,
        Hidden = 0x0040,
        Offline = 0x0100,
        ReadOnly = 0x0200,
        System = 0x0400,
        Temporary = 0x1000,
    }

    internal class Article : IArticle
    {
        public string Path { get; }
        public string Name { get; }

        public ArticleTypes ArticleType { get; }
        public ArticleAttributes ArticleAttribute { get; }

        public long Size { get; }

        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }
        public DateTime DateAccessed { get; }

        public BitmapImage Icon { get; }

        public Article(string path, WIN32_FIND_DATA data)
        {
            Path = path;
            Name = data.cFileName;
            ArticleType = IsDirectory(data) ? ArticleTypes.Folder : ArticleTypes.File;
            ArticleAttribute = ((IO.FileAttributes)data.dwFileAttributes).ToAttribute();
            Size = data.GetSize();
            DateCreated = ToDateTime(ref data.ftCreationTime);
            DateModified = ToDateTime(ref data.ftLastWriteTime);
            DateAccessed = ToDateTime(ref data.ftLastAccessTime);
        }
    }
}
