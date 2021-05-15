using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage.Search;

namespace Files.Filesystem.Search
{
    public class FolderSearchOption
    {
        public static FolderSearchOption Default { get; } = new FolderSearchOption();

        public string Query { get; set; }

        public FolderSearchFilterCollection Filters { get; } =
            new FolderSearchFilterCollection
            {
                new QueryFolderSearchFilter(),
                new DateFolderSearchFilter("creationDate", "Creation date"),
                new DateFolderSearchFilter("modificationDate", "modification Date")
            };

        private FolderSearchOption ()
        {
        }

        public QueryOptions ToQueryOptions()
        {
            var options = new QueryOptions{ FolderDepth = FolderDepth.Deep };

            options.IndexerOption = App.AppSettings.SearchUnindexedItems
                ? IndexerOption.DoNotUseIndexer
                : IndexerOption.OnlyUseIndexerAndOptimizeForIndexedProperties;

            options.UserSearchFilter = string.Join(' ', Filters
                .Select(filter => filter.ToAdvancedQuerySyntax())
                .Where(aqs => !string.IsNullOrEmpty(aqs))
            );

            options.SortOrder.Clear();
            options.SortOrder.Add(new SortEntry{ PropertyName = "System.Search.Rank", AscendingOrder = false });

            options.SetPropertyPrefetch(Windows.Storage.FileProperties.PropertyPrefetchOptions.None, null);
            options.SetThumbnailPrefetch(Windows.Storage.FileProperties.ThumbnailMode.ListView, 24,
                Windows.Storage.FileProperties.ThumbnailOptions.UseCurrentScale);

            return options;
        }
    }

    public interface IFolderSearchFilter
    {
        string ToAdvancedQuerySyntax();
    }

    public interface ICriteriaFolderSearchFilter : IFolderSearchFilter
    {
        string Key { get; }
    }

    public class FolderSearchFilterCollection : Collection<IFolderSearchFilter>, IFolderSearchFilter
    {
        public FolderSearchFilterCollection() : base()
        {
        }
        public FolderSearchFilterCollection(IList<IFolderSearchFilter> filters) : base(filters)
        {
        }

        public string ToAdvancedQuerySyntax() =>
            string.Join(' ', Items.Select(filter => filter.ToAdvancedQuerySyntax()));
    }

    public class QueryFolderSearchFilter : IFolderSearchFilter
    {
        public string Query { get; }

        public string ToAdvancedQuerySyntax() => Query ?? string.Empty;
    }

    public abstract class CriteriaFolderSearchFilter : ICriteriaFolderSearchFilter
    {
        public string Key { get; }
        public string Label { get; }

        public CriteriaFolderSearchFilter(string key, string label)
        {
            Key = key;
            Label = label;
        }

        public abstract string ToAdvancedQuerySyntax();
    }

    public class DateFolderSearchFilter : CriteriaFolderSearchFilter
    {
        public enum Periods : ushort
        {
            None,
            Custom,
            DayAgo,
            WeekAgo,
            MonthAgo,
            YearAgo,
        }

        public enum Comparators : ushort
        {
            None,
            After,
            Before,
            Between,
        }

        public Periods Period = Periods.None;
        public Comparators Comparator = Comparators.Between;

        public DateTimeOffset? MinDate = DateTimeOffset.MinValue;
        public DateTimeOffset? MaxDate = DateTimeOffset.MaxValue;

        public DateFolderSearchFilter(string key, string label) : base(key, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Period switch
            {
                Periods.Custom => ToAdvancedQuerySyntax_Custom(),
                Periods.DayAgo => $"{Key}:>System.StructuredQueryType.DateTime#LastDay",
                Periods.WeekAgo => $"{Key}:>System.StructuredQueryType.DateTime#LastWeek",
                Periods.MonthAgo => $"{Key}:>System.StructuredQueryType.DateTime#LastMonth",
                Periods.YearAgo => $"{Key}:>System.StructuredQueryType.DateTime#LastYear",
                _ => string.Empty
            };
        }
        private string ToAdvancedQuerySyntax_Custom()
        {
            return Comparator switch
            {
                Comparators.After =>
                    MinDate.HasValue ? $"{Key}:>={MinDate:yyyy/MM/dd}" : string.Empty,
                Comparators.Before =>
                    MaxDate.HasValue ? $"{Key}:<={MaxDate:yyyy/MM/dd}" : string.Empty,
                Comparators.Between =>
                    (MinDate.HasValue ? $"{Key}:>={MinDate:yyyy/MM/dd}" : string.Empty)
                    + (MinDate.HasValue && MaxDate.HasValue ? " " : string.Empty) +
                    (MaxDate.HasValue ? $"{Key}:<={MaxDate:yyyy/MM/dd}" : string.Empty),
                _ => string.Empty
            };
        }
    }
}
