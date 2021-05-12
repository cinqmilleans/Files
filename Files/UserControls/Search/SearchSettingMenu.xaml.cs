using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Files.UserControls.Search
{
    public sealed partial class SearchSettingMenu : UserControl
    {
        private readonly FilterDictionary Filters;

        public SearchSettingMenu()
        {
            this.InitializeComponent();

            var filters = new List<ICriteriaFilter>
            {
                new DateFilter("creationDate", "Date de création"),
                new StringFilter("artist", "Artist"),
                new ComparableFilter<ulong>("Size", "size"),
            };

            Filters = new FilterDictionary(filters.ToDictionary(filter => filter.Code, filter => (IFilter)filter));
        }

        private interface IFilter
        {
            string ToFilterString();
        }
        private interface ICriteriaFilter : IFilter
        {
            string Code { get; }
            string Label { get; }
        }

        private class FilterDictionary : Dictionary<string, IFilter>
        {
            public FilterDictionary() : base()
            {
            }
            public FilterDictionary(IDictionary<string, IFilter> dictionary) : base(dictionary)
            {
            }
            public FilterDictionary(int capacity) : base(capacity)
            {
            }
            public FilterDictionary(IEnumerable<KeyValuePair<string, IFilter>> collection) : base(collection)
            {
            }
            public FilterDictionary(IEqualityComparer<string> comparer) : base(comparer)
            {
            }
            public FilterDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
            {
            }
            public FilterDictionary(IDictionary<string, IFilter> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer)
            {
            }
            public FilterDictionary(IEnumerable<KeyValuePair<string, IFilter>> collection, IEqualityComparer<string> comparer) : base(collection, comparer)
            {
            }

            public string ToFilterString()
                => string.Join(' ', this.Values.Select(item => item.ToFilterString()));
        }

        private abstract class CriteriaFilter : ObservableObject, ICriteriaFilter
        {
            public string Code { get; }
            public string Label { get; }

            public CriteriaFilter(string code, string label)
            {
                Code = code;
                Label = label;
            }

            public abstract string ToFilterString();
        }

        private class ComparableFilter<T> : CriteriaFilter, ICriteriaFilter where T : IComparable<T>
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

            public ComparableFilter(string code, string label) : base(code, label)
            {
            }

            public override string ToFilterString()
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

        private class StringFilter : CriteriaFilter
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

            public StringFilter(string code, string label) : base(code, label)
            {
            }

            public override string ToFilterString()
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

        private class DateFilter : CriteriaFilter
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

            public DateFilter(string code, string label) : base(code, label)
            {
            }

            public override string ToFilterString()
            {
                return Comparator switch
                {
                    Comparators.None => string.Empty,

                    Comparators.EqualTo => $"{Code}:={MinDate:yyyy/MM/dd}",
                    Comparators.Before => $"{Code}:<={MaxDate:yyyy/MM/dd}",
                    Comparators.After => $"{Code}:>={MinDate:yyyy/MM/dd}",
                    Comparators.Between => $"{Code}:>={MinDate:yyyy/MM/dd} {Code}:<={MaxDate:yyyy/MM/dd}",

                    Comparators.DayAgo => $"{Code}:={DateTime.Now.Date.AddDays(-1):yyyy/MM/dd}",
                    Comparators.WeekAgo => $"{Code}:={DateTime.Now.Date.AddDays(-7):yyyy/MM/dd}",
                    Comparators.MonthAgo => $"{Code}:={DateTime.Now.Date.AddMonths(-1):yyyy/MM/dd}",
                    Comparators.YearAgo => $"{Code}:={DateTime.Now.Date.AddYears(-1):yyyy/MM/dd}",

                    _ => string.Empty
                };
            }
        }
    }
}
