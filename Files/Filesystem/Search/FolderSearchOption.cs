using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class FolderSearchOption
    {
        public FolderSearchFilterCollection Filters { get; }

        public static FolderSearchOption Default { get; } =
            new FolderSearchOption(
                new DateFolderSearchFilter("creationDate", "Creation date"),
                new DateFolderSearchFilter("modificationDate", "modification Date"),
                new ComparableFolderSearchFilter<int>("size", "Size"),
                new StringFolderSearchFilter("size", "Size")
            );

        private FolderSearchOption (params IFolderSearchFilter[] filters)
        {
            Filters = new FolderSearchFilterCollection(string.Empty, filters);
        }
    }

    public interface IFolderSearchFilter
    {
        string Key { get; }
        string ToAdvancedQuerySyntax();
    }

    public class FolderSearchFilterCollection : Collection<IFolderSearchFilter>, IFolderSearchFilter
    {
        public string Key { get; }

        public FolderSearchFilterCollection(string key) : base()
        {
            Key = key;
        }
        public FolderSearchFilterCollection(string key, IList<IFolderSearchFilter> filters) : base(filters)
        {
            Key = key;
        }

        public string ToAdvancedQuerySyntax() =>
            string.Join(' ', Items.Select(filter => filter.ToAdvancedQuerySyntax()));
    }

    public abstract class CriteriaFolderSearchFilter : IFolderSearchFilter
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

    public class ComparableFolderSearchFilter<T> : CriteriaFolderSearchFilter where T : IComparable<T>
    {
        public enum Comparators : ushort
        {
            None,
            EqualTo,
            LessThan,
            GreaterThan,
            Between,
        }

        public Comparators Comparator = Comparators.EqualTo;

        public T MinValue;
        public T MaxValue;

        public ComparableFolderSearchFilter(string key, string label) : base(key, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Comparator switch
            {
                Comparators.None => string.Empty,

                Comparators.EqualTo => $"{Key}:={MinValue}",
                Comparators.LessThan => $"{Key}:<={MaxValue}",
                Comparators.GreaterThan => $"{Key}:>={MinValue}",
                Comparators.Between => $"{Key}:>={MinValue} {Key}:<={MaxValue}",

                _ => string.Empty
            };
        }
    }

    public class StringFolderSearchFilter : CriteriaFolderSearchFilter
    {
        public enum Comparators : ushort
        {
            None,
            EqualTo,
            StartsWith,
            EndsWith,
            Contains,
        }

        public Comparators Comparator = Comparators.EqualTo;

        public string Value;

        public StringFolderSearchFilter(string key, string label) : base(key, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Comparator switch
            {
                Comparators.None => string.Empty,

                Comparators.EqualTo => $"{Key}:={Value}",
                Comparators.StartsWith => $"{Key}:={Value}",
                Comparators.EndsWith => $"{Key}:={Value}",
                Comparators.Contains => $"{Key}:={Value}",

                _ => string.Empty
            };
        }
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

        public Periods Period = Periods.MonthAgo;
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
