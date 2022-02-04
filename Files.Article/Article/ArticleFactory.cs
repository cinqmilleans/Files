using Files.Article.Extension;
using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Windows.UI.Xaml.Media.Imaging;
using static Files.Article.Helper.NativeFindStorageItemHelper;

namespace Files.Article.Article
{
    internal interface IArticleFactory
    {
        IArticle BuildArticle(string path);
        IArticle BuildArticle(string path, WIN32_FIND_DATA data);
    }

    internal class ArticleFactory : IArticleFactory
    {
        public IArticle BuildArticle(string path)
            => throw new NotImplementedException();

        public IArticle BuildArticle(string path, WIN32_FIND_DATA data)
        {
            return new Article
            {
                Path = path,
                Name = data.cFileName,
                ArticleType = IsDirectory(data) ? ArticleTypes.Folder : ArticleTypes.File,
                Size = data.GetSize(),
                DateCreated = ToDateTime(ref data.ftCreationTime),
                DateModified = ToDateTime(ref data.ftLastWriteTime),
                DateAccessed = ToDateTime(ref data.ftLastAccessTime),
            };
        }

        private static bool IsDirectory(WIN32_FIND_DATA data)
            => ((FileAttributes)data.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory;

        private static DateTime ToDateTime(ref FILETIME time)
        {
            FileTimeToSystemTime(ref time, out SYSTEMTIME systemTime);
            return systemTime.ToDateTime();
        }

        private class Article : IArticle
        {
            public string Path { get; set; }
            public string Name { get; set; }

            public ArticleTypes ArticleType { get; set; } = ArticleTypes.Unknown;

            public long? Size { get; set; }

            public DateTime? DateCreated { get; set; }
            public DateTime? DateModified { get; set; }
            public DateTime? DateAccessed { get; set; }

            public BitmapImage IconDefault { get; set; }
            public BitmapImage IconFile { get; set; }
            public BitmapImage IconOverlay { get; set; }
        }

        private class ShortcutArticle : Article, IShortcutArticle
        {
            public ShortcutTypes ShortcutType { get; set; } = ShortcutTypes.Unknown;

            public string TargetPath { get; set; }
            public string Arguments { get; set; }
            public string WorkingDirectory { get; set; }
        }
    }
}
