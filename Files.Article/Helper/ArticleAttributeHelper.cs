using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IO = System.IO;

namespace Files.Article.Article
{
    internal static class ArticleAttributeHelper
    {
        private static readonly IReadOnlyDictionary<IO.FileAttributes, FileAttributes> attributes
            = new ReadOnlyDictionary<IO.FileAttributes, FileAttributes>
            (
                new Dictionary<IO.FileAttributes, FileAttributes>
                {
                    [IO.FileAttributes.Archive] = FileAttributes.Archive,
                    [IO.FileAttributes.Compressed] = FileAttributes.Compressed,
                    [IO.FileAttributes.Device] = FileAttributes.Device,
                    [IO.FileAttributes.Directory] = FileAttributes.Directory,
                    [IO.FileAttributes.Encrypted] = FileAttributes.Encrypted,
                    [IO.FileAttributes.Hidden] = FileAttributes.Hidden,
                    [IO.FileAttributes.Offline] = FileAttributes.Offline,
                    [IO.FileAttributes.ReadOnly] = FileAttributes.ReadOnly,
                    [IO.FileAttributes.System] = FileAttributes.System,
                    [IO.FileAttributes.Temporary] = FileAttributes.Temporary,
                }
            );

        public static FileAttributes ToAttribute(this IO.FileAttributes fileAttributes)
            => attributes
                .Where(attribute => fileAttributes.HasFlag(attribute.Key))
                .Select(attribute => attribute.Value)
                .Aggregate((result, attribute) => result | attribute);
    }
}
