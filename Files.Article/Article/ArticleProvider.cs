using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files.Article.Article
{
    public interface IArticleProvider
    {
        IEnumerable<IArticle> EnumerateArticle();
    }

    public class ArticleProvider : IArticleProvider
    {
        private readonly IArticleFactory factory = new ArticleFactory();

        public string ParentPath { get; set; }

        public bool ShowHiddens { get; set; } = false;
        public bool ShowSystems { get; set; } = false;
        public bool ShowFolderSize { get; set; } = false;

        public uint CountLimit { get; set; } = uint.MaxValue;

        public CancellationToken CancellationToken { get; set; }

        public IEnumerable<IArticle> EnumerateArticle()
        {

        }
    }
}
