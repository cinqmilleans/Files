using Microsoft.Toolkit.Mvvm.DependencyInjection;
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
    public interface ISearchFilterCollection : IList<ISearchFilter>, IMultiSearchFilter, INotifyCollectionChanged
    {
    }

    [SearchHeader]
    public class GroupAndHeader : ISearchHeader
    {
        public SearchKeys Key => SearchKeys.GroupAnd;

        public string Glyph => "\uEC26";
        public string Label => "And".GetLocalized();
        public string Description => "SearchAndFilterCollection_Description".GetLocalized();

        public ISearchFilter CreateFilter() => new SearchFilterCollection(Key);
    }

    [SearchHeader]
    public class GroupOrHeader : ISearchHeader
    {
        public SearchKeys Key => SearchKeys.GroupOr;

        public string Glyph => "\uEC26";
        public string Label => "Or".GetLocalized();
        public string Description => "SearchOrFilterCollection_Description".GetLocalized();

        public ISearchFilter CreateFilter() => new SearchFilterCollection(Key);
    }

    [SearchHeader]
    public class GroupNotHeader : ISearchHeader
    {
        public SearchKeys Key => SearchKeys.GroupNot;

        public string Glyph => "\uEC26";
        public string Label => "Not".GetLocalized();
        public string Description => "SearchNotFilterCollection_Description".GetLocalized();

        public ISearchFilter CreateFilter() => new SearchFilterCollection(Key);
    }

    public class SearchFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public SearchKeys Key
        {
            get => header.Key;
            set
            {

                if (value is not SearchKeys.GroupAnd and not SearchKeys.GroupOr and not SearchKeys.GroupNot)
                {
                    throw new ArgumentException();
                }
                if (header.Key != value)
                {
                    header = GetHeader(Key);
                    OnPropertyChanged();
                }
            }
        }

        private ISearchHeader header;
        public ISearchHeader Header => header;

        public bool IsEmpty => !this.Any();

        public IEnumerable<ISearchTag> Tags => this.Any()
            ? new ISearchTag[1] { new Tag(this) }
            : Enumerable.Empty<ISearchTag>();

        public SearchFilterCollection(SearchKeys key) => header = GetHeader(key);
        public SearchFilterCollection(SearchKeys key, IList<ISearchFilter> filters) : base(filters) => header = GetHeader(key);

        public string ToAdvancedQuerySyntax()
        {
            var queries = this
                .Where(filter => filter is not null)
                .Select(filter => (filter.ToAdvancedQuerySyntax() ?? string.Empty).Trim())
                .Where(query => !string.IsNullOrEmpty(query));

            return Key switch
            {
                SearchKeys.GroupAnd => string.Join(' ', queries.Select(query => query.Contains(' ') ? $"({query})" : query)),
                SearchKeys.GroupOr => string.Join(" OR ", queries.Select(query => query.Contains(' ') ? $"({query})" : query)),
                SearchKeys.GroupNot => string.Join(' ', queries.Select(query => $"NOT({query})")),
                _ => throw new InvalidOperationException(),
            };
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(Tags));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        private static ISearchHeader GetHeader (SearchKeys key)
        {
            var provider = Ioc.Default.GetService<ISearchHeaderProvider>();
            return provider.GetHeader(key);
        }

        private class Tag : ISearchTag
        {
            ISearchFilter ISearchTag.Filter => Filter;
            public ISearchFilterCollection Filter { get; }

            public string Title => string.Empty;
            public string Parameter => $"{Filter.Count} items";

            public Tag(ISearchFilterCollection filter) => Filter = filter;

            public void Delete() => (Filter as ISearchFilter).Clear();
        }
    }
}
