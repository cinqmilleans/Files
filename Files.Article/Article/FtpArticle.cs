using Files.Helpers;
using FluentFTP;
using System;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Article.Article
{
    internal class FtpArticle : IArticle
    {
        public string Path { get; }
        public string Name { get; }

        public ArticleTypes ArticleType { get; }
        public ArticleAttributes ArticleAttribute { get; }

        public long Size { get; }

        public DateTime DateCreated { get; }
        public DateTime DateModified { get; }
        public DateTime DateAccessed => DateModified;

        public BitmapImage Icon { get; }

        internal FtpArticle(FtpListItem item, string folder)
        {
            Path = PathNormalization.Combine(folder, item.Name);
            Name = item.Name;
            ArticleType = item.Type == FtpFileSystemObjectType.File ? ArticleTypes.File : ArticleTypes.Folder;
            Size = item.Size;
            DateCreated = Clean(item.RawCreated);
            DateModified = Clean(item.RawModified);
        }

        private static DateTime Clean(DateTime date)
            => date < DateTime.FromFileTimeUtc(0) ? DateTime.MinValue : date;
    }
}
