using Files.Article.Article;
using System;

namespace Files.Article.ViewModel
{
    public interface IArticleViewModelFactory
    {
        IArticleViewModel Build(object article);
    }

    internal class ArticleViewModelFactory : IArticleViewModelFactory
    {
        public IArticleViewModel Build(object article) => article switch
        {
            ILibrary l => new LibraryArticleViewModel(l),
            IShortcutArticle s => new ShortcutArticleViewModel(s),
            IArticle a => a.ArticleType switch
            {
                ArticleTypes.File => new FileViewModel(a),
                ArticleTypes.Folder => new FolderViewModel(a),
                _ => throw new ArgumentException(),
            },
            _ => throw new ArgumentException(),
        };
    }
}
