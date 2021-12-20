using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings
    {
        ObservableCollection<string> PinnedKeys { get; }

        ISearchLocation Location { get; }
        ISearchFilterCollection Filter { get; }
    }

    public interface ISearchLocation : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }
    }

    public interface ISearchFilter
    {
        string ToAdvancedQuerySyntax();
    }

    public interface ISearchFilterCollection : IList<ISearchFilter>, ISearchFilter, IHeader, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }

    public interface IDateRangeFilter : ISearchFilter, IHeader
    {
        DateRange Range { get; }
    }
    public interface ISizeRangeFilter : ISearchFilter, IHeader
    {
        SizeRange Range { get; }
    }

    public interface IHeader
    {
        string Glyph { get; }
        string Title { get; }
        string Description { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public ObservableCollection<string> PinnedKeys { get; }

        public ISearchLocation Location { get; } = new SearchLocation();
        public ISearchFilterCollection Filter { get; }

        public SearchSettings()
        {
            var pinnedKeys = new string[] { /*"size", "modified"*/ };

            var manager = Ioc.Default.GetService<ISearchFilterManager>();

            PinnedKeys = new ObservableCollection<string>(pinnedKeys);
            Filter = new AndFilterCollection(pinnedKeys.Select(key => manager.GetFilter(key)));
        }
    }

    public class SearchLocation : ObservableObject, ISearchLocation
    {
        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }
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

    public abstract class DateRangeFilter : IDateRangeFilter
    {
        public virtual string Glyph => "\uEC92";
        public abstract string Title { get; }
        public abstract string Description { get; }

        public DateRange Range { get; }

        protected abstract string QueryKey { get; }

        public DateRangeFilter() => Range = DateRange.Always;
        public DateRangeFilter(DateRange range) => Range = range;

        public string ToAdvancedQuerySyntax()
        {
            var (_, direction, minValue, maxValue) = Range;

            return direction switch
            {
                RangeDirections.EqualTo => $"{QueryKey}:={minValue:yyyy-MM-dd}",
                RangeDirections.LessThan => $"{QueryKey}:<={maxValue:yyyy-MM-dd}",
                RangeDirections.GreaterThan => $"{QueryKey}:>={minValue:yyyy-MM-dd}",
                RangeDirections.Between => $"{QueryKey}:{minValue:yyyy-MM-dd}..{maxValue:yyyy-MM-dd}",
                _ => string.Empty,
            };
        }
    }
    [SearchFilter("created")]
    public class CreatedFilter : DateRangeFilter
    {
        public override string Title => "Created".GetLocalized();
        public override string Description => "SearchCreatedFilter_Description".GetLocalized();
        protected override string QueryKey => "System.ItemDate";

        public CreatedFilter() : base() {}
        public CreatedFilter(DateRange range) : base(range) {}
    }
    [SearchFilter("modified")]
    public class ModifiedFilter : DateRangeFilter
    {
        public override string Title => "Modified".GetLocalized();
        public override string Description => "SearchModifiedFilter_Description".GetLocalized();
        protected override string QueryKey => "System.DateModified";

        public ModifiedFilter() : base() {}
        public ModifiedFilter(DateRange range) : base(range) {}
    }
    [SearchFilter("accessed")]
    public class AccessedFilter : DateRangeFilter
    {
        public override string Title => "Accessed".GetLocalized();
        public override string Description => "SearchAccessedFilter_Description".GetLocalized();
        protected override string QueryKey => "System.DateAccessed";

        public AccessedFilter() : base() {}
        public AccessedFilter(DateRange range) : base(range) {}
    }

    [SearchFilter("size")]
    public class SizeRangeFilter : ISizeRangeFilter
    {
        public string Glyph => "\uE2B2";
        public string Title => "Size".GetLocalized();
        public string Description => "SearchSizeFilter_Description".GetLocalized();

        public SizeRange Range { get; }

        public SizeRangeFilter() => Range = SizeRange.All;
        public SizeRangeFilter(SizeRange range) => Range = range;

        public string ToAdvancedQuerySyntax()
        {
            var (_, direction, minValue, maxValue) = Range;

            return direction switch
            {
                RangeDirections.EqualTo => $"System.Size:={minValue.Bytes}",
                RangeDirections.LessThan => $"System.Size:<={maxValue.Bytes}",
                RangeDirections.GreaterThan => $"System.Size:>={minValue.Bytes}",
                RangeDirections.Between => $"System.Size:{minValue.Bytes}..{maxValue.Bytes}",
                _ => string.Empty,
            };
        }
    }
}
