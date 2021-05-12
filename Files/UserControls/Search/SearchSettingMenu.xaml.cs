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
        public SearchSettingMenu()
        {
            this.InitializeComponent();
        }

        private readonly FilterCollection Filters = new FilterCollection
        {
            new DateFilter("Date de création", "creationDate"),
            new StringFilter("Artist", "artist"),
            new ComparableFilter<ulong>("Size", "size"),
        };

        private interface IFilter
        {
            string ToFilterString();
        }
        private interface ICriteriaFilter : IFilter
        {
            string Label { get; }
        }

        private class FilterCollection : Collection<IFilter>, IFilter
        {
            public FilterCollection() : base()
            {
            }
            public FilterCollection(IList<IFilter> filters) : base(filters)
            {
            }

            public string ToFilterString()
                => string.Join(' ', Items.Select(item => item.ToFilterString()));
        }

        private class ComparableFilter<T> : ObservableObject, ICriteriaFilter where T : IComparable<T>
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

            private T minValue;
            private T maxValue;

            private readonly string field;

            public string Label { get; }

            public Comparators Comparator
            {
                get => comparator;
                set => SetProperty(ref comparator, value);
            }

            public T MinValue
            {
                get => minValue;
                set => SetProperty(ref minValue, value);
            }
            public T MaxValue
            {
                get => maxValue;
                set => SetProperty(ref maxValue, value);
            }

            public ComparableFilter(string label, string field)
            {
                Label = label;
                this.field = field;
            }

            public string ToFilterString()
            {
                return Comparator switch
                {
                    Comparators.None => string.Empty,

                    Comparators.EqualTo => $"{field}:={minValue}",
                    Comparators.LessThan => $"{field}:<={maxValue}",
                    Comparators.GreaterThan => $"{field}:>={minValue}",
                    Comparators.Between => $"{field}:>={minValue} {field}:<={maxValue}",

                    _ => string.Empty
                };
            }
        }

        private class StringFilter : ObservableObject, ICriteriaFilter
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

            private string value;

            private readonly string field;

            public string Label { get; }

            public Comparators Comparator
            {
                get => comparator;
                set => SetProperty(ref comparator, value);
            }

            public string Value
            {
                get => value;
                set => SetProperty(ref this.value, value);
            }

            public StringFilter(string label, string field)
            {
                Label = label;
                this.field = field;
            }

            public string ToFilterString()
            {
                return Comparator switch
                {
                    Comparators.None => string.Empty,

                    Comparators.EqualTo => $"{field}:={value}",
                    Comparators.StartsWith => $"{field}:={value}",
                    Comparators.EndsWith => $"{field}:={value}",
                    Comparators.Contains => $"{field}:={value}",

                    _ => string.Empty
                };
            }
        }

        private class DateFilter : ObservableObject, ICriteriaFilter
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

            private DateTime minDate = DateTime.MinValue;
            private DateTime maxDate = DateTime.MaxValue;

            private readonly string field;

            public string Label { get; }

            public Comparators Comparator
            {
                get => comparator;
                set => SetProperty(ref comparator, value);
            }

            public DateTime MinDate
            {
                get => minDate;
                set => SetProperty(ref minDate, value);
            }
            public DateTime MaxDate
            {
                get => maxDate;
                set => SetProperty(ref maxDate, value);
            }

            public DateFilter(string label, string field)
            {
                Label = label;
                this.field = field;
            }

            public string ToFilterString()
            {
                return Comparator switch
                {
                    Comparators.None => string.Empty,

                    Comparators.EqualTo => $"{field}:={minDate:yyyy/MM/dd}",
                    Comparators.Before => $"{field}:<={maxDate:yyyy/MM/dd}",
                    Comparators.After => $"{field}:>={minDate:yyyy/MM/dd}",
                    Comparators.Between => $"{field}:>={minDate:yyyy/MM/dd} {field}:<={maxDate:yyyy/MM/dd}",

                    Comparators.DayAgo => $"{field}:={DateTime.Now.Date.AddDays(-1):yyyy/MM/dd}",
                    Comparators.WeekAgo => $"{field}:={DateTime.Now.Date.AddDays(-7):yyyy/MM/dd}",
                    Comparators.MonthAgo => $"{field}:={DateTime.Now.Date.AddMonths(-1):yyyy/MM/dd}",
                    Comparators.YearAgo => $"{field}:={DateTime.Now.Date.AddYears(-1):yyyy/MM/dd}",

                    _ => string.Empty
                };
            }
        }
    }
}
