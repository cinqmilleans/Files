using Microsoft.Toolkit.Mvvm.ComponentModel;
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

    public abstract class CriteriaFolderSearchFilter : ObservableObject, IFolderSearchFilter
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

        private Comparators comparator = Comparators.EqualTo;
        public Comparators Comparator
        {
            get => comparator;
            set => SetProperty(ref comparator, value);
        }

        private T minValue;
        public T MinValue
        {
            get => minValue;
            set => SetProperty(ref minValue, value);
        }

        private T maxValue;
        public T MaxValue
        {
            get => maxValue;
            set => SetProperty(ref maxValue, value);
        }

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

        private Comparators comparator = Comparators.EqualTo;
        public Comparators Comparator
        {
            get => comparator;
            set => SetProperty(ref comparator, value);
        }

        private string value;
        public string Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

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

        private Periods period = Periods.None;
        public Periods Period
        {
            get => period;
            set => SetProperty(ref period, value);
        }

        private Comparators comparator = Comparators.None;
        public Comparators Comparator
        {
            get => comparator;
            set => SetProperty(ref comparator, value);
        }

        private DateTimeOffset? minDate;
        public DateTimeOffset? MinDate
        {
            get => minDate;
            set => SetProperty(ref minDate, value > MinDate ? value : null);
        }

        private DateTimeOffset? maxDate;
        public DateTimeOffset? MaxDate
        {
            get => maxDate;
            set => SetProperty(ref maxDate, value < MaxDate ? value : null);
        }

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
