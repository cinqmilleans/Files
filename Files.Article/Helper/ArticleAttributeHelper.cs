using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Files.Article.Article
{
    /*internal static class ArticleAttributeHelper
    {
        private static readonly IReadOnlyDictionary<FileAttributes, ArticleAttributes> attributes
            = new ReadOnlyDictionary<FileAttributes, ArticleAttributes>
            (
                new Dictionary<FileAttributes, ArticleAttributes>
                {
                    [FileAttributes.Archive] = ArticleAttributes.Archive,
                    [FileAttributes.Compressed] = ArticleAttributes.Compressed,
                    [FileAttributes.Device] = ArticleAttributes.Device,
                    [FileAttributes.Directory] = ArticleAttributes.Directory,
                    [FileAttributes.Encrypted] = ArticleAttributes.Encrypted,
                    [FileAttributes.Hidden] = ArticleAttributes.Hidden,
                    [FileAttributes.Offline] = ArticleAttributes.Offline,
                    [FileAttributes.ReadOnly] = ArticleAttributes.ReadOnly,
                    [FileAttributes.System] = ArticleAttributes.System,
                    [FileAttributes.Temporary] = ArticleAttributes.Temporary,
                }
            );

        public static ArticleAttributes ToAttribute(this FileAttributes fileAttributes)
            => attributes
                .Where(attribute => fileAttributes.HasFlag(attribute.Key))
                .Select(attribute => attribute.Value)
                .Aggregate((result, attribute) => result | attribute);
    }*/
}
