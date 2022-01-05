using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Files.Filesystem.Search
{
    public enum GroupAggregates : ushort { And, Or, Not }

    public interface IGroupHeader : ISearchHeader
    {
        GroupAggregates Aggregate { get; }

        new ISearchFilterCollection GetFilter();

        string ToAdvancedQuerySyntax(IEnumerable<ISearchFilter> filters);
    }

    public interface ISearchFilterCollection : IList<ISearchFilter>, ISearchFilter, INotifyCollectionChanged
    {
    }

    [SearchHeader]
    public class AndHeader : IGroupHeader
    {
        public GroupAggregates Aggregate => GroupAggregates.And;

        public string Key => "and";
        public string Glyph => "\uEC26";
        public string Title => "And".GetLocalized();
        public string Description => string.Empty;

        ISearchFilter ISearchHeader.GetFilter() => GetFilter();
        public ISearchFilterCollection GetFilter() => new SearchFilterCollection(Aggregate);

        public string ToAdvancedQuerySyntax(IEnumerable<ISearchFilter> filters)
        {
            var queries = filters
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => query.Contains(' ') ? $"({query})" : query);
            return string.Join(' ', queries);
        }
    }

    [SearchHeader]
    public class OrHeader : IGroupHeader
    {
        public GroupAggregates Aggregate => GroupAggregates.Or;

        public string Key => "or";
        public string Glyph => "\uEC26";
        public string Title => "Or".GetLocalized();
        public string Description => string.Empty;

        ISearchFilter ISearchHeader.GetFilter() => GetFilter();
        public ISearchFilterCollection GetFilter() => new SearchFilterCollection(Aggregate);

        public string ToAdvancedQuerySyntax(IEnumerable<ISearchFilter> filters)
        {
            var queries = filters
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => query.Contains(' ') ? $"({query})" : query);
            return string.Join(" OR ", queries);
        }
    }

    [SearchHeader]
    public class NotHeader : IGroupHeader
    {
        public GroupAggregates Aggregate => GroupAggregates.Not;

        public string Key => "not";
        public string Glyph => "\uEC26";
        public string Title => "Not".GetLocalized();
        public string Description => string.Empty;

        ISearchFilter ISearchHeader.GetFilter() => GetFilter();
        public ISearchFilterCollection GetFilter() => new SearchFilterCollection(Aggregate);

        public string ToAdvancedQuerySyntax(IEnumerable<ISearchFilter> filters)
        {
            var queries = filters
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query))
                .Select(query => $"NOT({query})");
            return string.Join(" ", queries);
        }
    }

    public class SearchFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public GroupAggregates Aggregate
        {
            get => Header.Aggregate;
            set
            {
                if (value != Aggregate)
                {
                    Header = value switch
                    {
                        GroupAggregates.And => new AndHeader(),
                        GroupAggregates.Or => new OrHeader(),
                        GroupAggregates.Not => new NotHeader(),
                        _ => throw new NotSupportedException(),
                    };
                    OnPropertyChanged();
                }
            }
        }

        ISearchHeader ISearchFilter.Header => Header;
        public IGroupHeader Header = new AndHeader();

        public IEnumerable<ISearchTag> Tags => this.Any()
            ? new ISearchTag[1] { new Tag(this) }
            : Enumerable.Empty<ISearchTag>();

        public SearchFilterCollection()
            : base() {}
        public SearchFilterCollection(GroupAggregates aggregate)
            : base() => Aggregate = aggregate;
        public SearchFilterCollection(IList<ISearchFilter> filters)
            : base(filters) {}
        public SearchFilterCollection(GroupAggregates aggregate, IList<ISearchFilter> filters)
            : base(filters) => Aggregate = aggregate;

        public string ToAdvancedQuerySyntax() => Header.ToAdvancedQuerySyntax(this);

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            OnPropertyChanged(nameof(Tags));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        private class Tag : ISearchTag
        {
            ISearchFilter ISearchTag.Filter => Filter;
            public ISearchFilterCollection Filter { get; }

            public string Title => string.Empty;
            public string Parameter => $"{Filter.Count} items";

            public Tag(ISearchFilterCollection filter) => Filter = filter;

            public void Delete() => Filter.Clear();
        }
    }
}
