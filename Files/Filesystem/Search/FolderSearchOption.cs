using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class FolderSearchOption
    {
        public FolderSearchCriteriaSet Filters = new FolderSearchCriteriaSet();
    }

    public class FolderSearchCriteriaSet : IFolderSearchFilter
    {
        private readonly IFolderSearchCriteria[] filters;

        public DateFolderSearchCriteria CreationDate { get; } =
            new DateFolderSearchCriteria("creationDate", "Creation date");

        public DateFolderSearchCriteria ModificationDate { get; } =
            new DateFolderSearchCriteria("modificationDate", "modification Date");

        public ComparableFolderSearchCriteria<int> Size { get; } =
            new ComparableFolderSearchCriteria<int>("size", "Size");

        public StringFolderSearchCriteria Artist { get; } =
            new StringFolderSearchCriteria("size", "Size");

        public FolderSearchCriteriaSet()
        {
            filters = new IFolderSearchCriteria[]
            {
                CreationDate,
                ModificationDate,
                Size,
                Artist,
            };
        }

        public string ToAdvancedQuerySyntax() =>
            string.Join(' ', filters.Select(filter => filter.ToAdvancedQuerySyntax()));
    }

    public interface IFolderSearchFilter
    {
        string ToAdvancedQuerySyntax();
    }

    public interface IFolderSearchCriteria : IFolderSearchFilter
    {
        string Code { get; }
        string Label { get; }
    }

    public abstract class FolderSearchCriteria : ObservableObject, IFolderSearchCriteria
    {
        public string Code { get; }
        public string Label { get; }

        public FolderSearchCriteria(string code, string label)
        {
            Code = code;
            Label = label;
        }

        public abstract string ToAdvancedQuerySyntax();
    }

    public class ComparableFolderSearchCriteria<T> : FolderSearchCriteria where T : IComparable<T>
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

        public ComparableFolderSearchCriteria(string code, string label) : base(code, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Comparator switch
            {
                Comparators.None => string.Empty,

                Comparators.EqualTo => $"{Code}:={MinValue}",
                Comparators.LessThan => $"{Code}:<={MaxValue}",
                Comparators.GreaterThan => $"{Code}:>={MinValue}",
                Comparators.Between => $"{Code}:>={MinValue} {Code}:<={MaxValue}",

                _ => string.Empty
            };
        }
    }

    public class StringFolderSearchCriteria : FolderSearchCriteria
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

        public StringFolderSearchCriteria(string code, string label) : base(code, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Comparator switch
            {
                Comparators.None => string.Empty,

                Comparators.EqualTo => $"{Code}:={Value}",
                Comparators.StartsWith => $"{Code}:={Value}",
                Comparators.EndsWith => $"{Code}:={Value}",
                Comparators.Contains => $"{Code}:={Value}",

                _ => string.Empty
            };
        }
    }

    public class DateFolderSearchCriteria : FolderSearchCriteria
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

        public DateFolderSearchCriteria(string code, string label) : base(code, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Period switch
            {
                Periods.Custom => ToAdvancedQuerySyntax_Custom(),
                Periods.DayAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastDay",
                Periods.WeekAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastWeek",
                Periods.MonthAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastMonth",
                Periods.YearAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastYear",
                _ => string.Empty
            };
        }
        private string ToAdvancedQuerySyntax_Custom()
        {
            return Comparator switch
            {
                Comparators.After =>
                    MinDate.HasValue ? $"{Code}:>={MinDate:yyyy/MM/dd}" : string.Empty,
                Comparators.Before =>
                    MaxDate.HasValue ? $"{Code}:<={MaxDate:yyyy/MM/dd}" : string.Empty,
                Comparators.Between =>
                    (MinDate.HasValue ? $"{Code}:>={MinDate:yyyy/MM/dd}" : string.Empty)
                    + (MinDate.HasValue && MaxDate.HasValue ? " " : string.Empty) +
                    (MaxDate.HasValue ? $"{Code}:<={MaxDate:yyyy/MM/dd}" : string.Empty),
                _ => string.Empty
            };
        }
    }
}
