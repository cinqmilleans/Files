using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        ISearchLocation Location { get; }
        ISearchFilterCollection Filters { get; }
    }

    public interface ISearchLocation : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }
    }

    public interface ISearchFilter
    {
        string Key { get; }
        string AdvancedQuerySyntax { get; }
    }

    public interface ISearchFilterCollection : IList<ISearchFilter>, ISearchFilter, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }

    public interface IDateRangeFilter : ISearchFilter
    {
        DateRange Range { get; }
    }
    public interface ISizeRangeFilter : ISearchFilter
    {
        SizeRange Range { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public ISearchLocation Location { get; } = new SearchLocation();
        public ISearchFilterCollection Filters { get; } = new AndFilterCollection();
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

    public class AndFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Key => "and";

        public string AdvancedQuerySyntax => string.Join(' ', this
            .Select(filter => (filter?.AdvancedQuerySyntax ?? string.Empty).Trim())
            .Where(query => !string.IsNullOrEmpty(query))
            .Select(query => query.Contains(' ') ? $"({query})" : query)
        );

        public AndFilterCollection() : base() {}
        public AndFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public AndFilterCollection(IList<ISearchFilter> filters) : base(filters) {}
    }
    public class OrFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Key => "or";

        public string AdvancedQuerySyntax => string.Join(" OR ", this
            .Select(filter => (filter?.AdvancedQuerySyntax ?? string.Empty).Trim())
            .Where(query => !string.IsNullOrEmpty(query))
            .Select(query => query.Contains(' ') ? $"({query})" : query)
        );

        public OrFilterCollection() : base() {}
        public OrFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public OrFilterCollection(IList<ISearchFilter> filters) : base(filters) {}

    }
    public class NotFilterCollection : ObservableCollection<ISearchFilter>, ISearchFilterCollection
    {
        public string Key => "not";

        public NotFilterCollection() : base() {}
        public NotFilterCollection(IEnumerable<ISearchFilter> filters) : base(filters) {}
        public NotFilterCollection(IList<ISearchFilter> filters) : base(filters) {}

        public string AdvancedQuerySyntax => string.Join(' ', this
            .Select(filter => (filter?.AdvancedQuerySyntax ?? string.Empty).Trim())
            .Where(query => !string.IsNullOrEmpty(query))
            .Select(query => $"NOT({query})")
        );
    }

    public abstract class DateRangeFilter : IDateRangeFilter
    {
        public abstract string Key { get; }

        public DateRange Range { get; }

        protected abstract string QueryKey { get; }

        public DateRangeFilter() => Range = DateRange.Always;
        public DateRangeFilter(DateRange range) => Range = range;

        public string AdvancedQuerySyntax
        {
            get
            {
                var (min, max) = Range;
                bool hasMin = min > Date.MinValue;
                bool hasMax = max < Date.Today;

                return (hasMin, hasMax) switch
                {
                    (false, false) => string.Empty,
                    _ when min == max => $"{QueryKey}:={min:yyyy-MM-dd}",
                    (false, _) => $"{QueryKey}:<={max:yyyy-MM-dd}",
                    (_, false) => $"{QueryKey}:>={min:yyyy-MM-dd}",
                    _ => $"{QueryKey}:{min:yyyy-MM-dd}..{max:yyyy-MM-dd}"
                };
            }
        }
    }
    public class CreatedFilter : DateRangeFilter
    {
        public override string Key => "created";
        protected override string QueryKey => "System.ItemDate";

        public CreatedFilter() : base() {}
        public CreatedFilter(DateRange range) : base(range) {}
    }
    public class ModifiedFilter : DateRangeFilter
    {
        public override string Key => "modified";
        protected override string QueryKey => "System.DateModified";

        public ModifiedFilter() : base() {}
        public ModifiedFilter(DateRange range) : base(range) {}
    }
    public class AccessedFilter : DateRangeFilter
    {
        public override string Key => "accessed";
        protected override string QueryKey => "System.DateAccessed";

        public AccessedFilter() : base() {}
        public AccessedFilter(DateRange range) : base(range) {}
    }

    public class SizeRangeFilter : ISizeRangeFilter
    {
        public string Key => "size";

        public SizeRange Range { get; }

        public SizeRangeFilter() => Range = SizeRange.All;
        public SizeRangeFilter(SizeRange range) => Range = range;

        public string AdvancedQuerySyntax
        {
            get
            {
                var (min, max) = Range;
                bool hasMin = min > Size.MinValue;
                bool hasMax = max < Size.MaxValue;

                return (hasMin, hasMax) switch
                {
                    (false, false) => string.Empty,
                    _ when min == max => $"System.Size:={min.Bytes}",
                    (false, _) => $"System.Size:<={max.Bytes}",
                    (_, false) => $"System.Size:>={min.Bytes}",
                    _ => $"System.Size:{min.Bytes}..{max.Bytes}"
                };
            }
        }
    }
}