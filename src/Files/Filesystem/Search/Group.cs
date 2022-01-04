using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchFilterCollection : IList<ISearchFilter>, ISearchFilter, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }

    [SearchFilter("and")]
    public class AndFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Glyph => "\uEC26";
        public string Title => "And".GetLocalized();
        public string Description => "SearchAndFilterCollection_Description".GetLocalized();

        public AndFilterCollection() : base() {}
        public AndFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public AndFilterCollection(IList<ISearchFilter> filters) : base(filters) {}

        public string ToAdvancedQuerySyntax()
        {
            var queries = this
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => query.Contains(' ') ? $"({query})" : query);
            return string.Join(' ', queries);
        }
    }

    [SearchFilter("or")]
    public class OrFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Glyph => "\uEC26";
        public string Title => "Or".GetLocalized();
        public string Description => "SearchOrFilterCollection_Description".GetLocalized();

        public OrFilterCollection() : base() {}
        public OrFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public OrFilterCollection(IList<ISearchFilter> filters) : base(filters) {}

        public string ToAdvancedQuerySyntax()
        {
            var queries = this
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => query.Contains(' ') ? $"({query})" : query);
            return string.Join(" OR ", queries);
        }
    }

    [SearchFilter("not")]
    public class NotFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Glyph => "\uEC26";
        public string Title => "Not".GetLocalized();
        public string Description => "SearchNotFilterCollection_Description".GetLocalized();

        public NotFilterCollection() : base() {}
        public NotFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public NotFilterCollection(IList<ISearchFilter> filters) : base(filters) {}

        public string ToAdvancedQuerySyntax()
        {
            var queries = this
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => $"NOT({query})");
            return string.Join(" ", queries);
        }
    }
}
