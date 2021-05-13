using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class FolderSearchOption
    {
        public FolderSearchCriteriaDictionary Filters;

        public FolderSearchOption()
        {
            var filters = CreateCriteria().ToDictionary(criteria => criteria.Code, criteria => criteria);
            Filters = new FolderSearchCriteriaDictionary(filters);
        }

        private List<IFolderSearchCriteria> CreateCriteria()
        {
            return new List<IFolderSearchCriteria>
            {
                new DateFolderSearchCriteria("creationDate", "Creation date"),
                new DateFolderSearchCriteria("modificationDate", "modification Date"),
                new ComparableFolderSearchCriteria<int>("size", "Size"),
                new StringFolderSearchCriteria("artist", "Artist"),
            };
        }
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

    public class FolderSearchCriteriaDictionary : ReadOnlyDictionary<string,IFolderSearchCriteria>, IFolderSearchFilter
    {
        public FolderSearchCriteriaDictionary(IDictionary<string, IFolderSearchCriteria> dictionary) : base(dictionary)
        {
        }

        public string ToAdvancedQuerySyntax() => string.Join(' ', this.Values.Select(filter => filter.ToAdvancedQuerySyntax()));
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
        public enum Comparators : ushort
        {
            None,
            EqualTo,
            Before,
            After,
            Between,
            DayAgo,
            WeekAgo,
            MonthAgo,
            YearAgo,
        }

        private Comparators comparator = Comparators.EqualTo;
        public Comparators Comparator
        {
            get => comparator;
            set => SetProperty(ref comparator, value);
        }

        private DateTime maxDate = DateTime.MaxValue;
        public DateTime MinDate
        {
            get => minDate;
            set => SetProperty(ref minDate, value);
        }

        private DateTime minDate = DateTime.MinValue;
        public DateTime MaxDate
        {
            get => maxDate;
            set => SetProperty(ref maxDate, value);
        }

        public DateFolderSearchCriteria(string code, string label) : base(code, label)
        {
        }

        public override string ToAdvancedQuerySyntax()
        {
            return Comparator switch
            {
                Comparators.None => string.Empty,

                Comparators.EqualTo => $"{Code}:={MinDate:yyyy/MM/dd}",
                Comparators.Before => $"{Code}:<={MaxDate:yyyy/MM/dd}",
                Comparators.After => $"{Code}:>={MinDate:yyyy/MM/dd}",
                Comparators.Between => $"{Code}:>={MinDate:yyyy/MM/dd} {Code}:<={MaxDate:yyyy/MM/dd}",

                Comparators.DayAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastDay",
                Comparators.WeekAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastWeek",
                Comparators.MonthAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastMonth",
                Comparators.YearAgo => $"{Code}:>System.StructuredQueryType.DateTime#LastYear",

                _ => string.Empty
            };
        }
    }
}
