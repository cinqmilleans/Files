using Files.Enums;
using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class YearSearchOptionValue : ISearchOptionValue
    {
        public ushort Year { get; }
        public string Text { get; }
        public string Label { get; }

        public YearSearchOptionValue(ushort year)
        {
            Year = year;
            Text = $"{year}";
            Label = $"Year {year}";
        }
    }

    public class YearSearchOptionFormat : ISearchOptionFormat
    {
        public ushort MinYear { get; set; } = 1900;
        public ushort MaxYear { get; set; } = 2200;

        public YearSearchOptionFormat()
        {
        }
        public YearSearchOptionFormat(ushort minYear, ushort maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;
        }

        public bool CanParseValue(string value)
            => ushort.TryParse(value, out ushort year) && year >= MinYear && year <= MaxYear;

        public ISearchOptionValue ParseValue(string value)
            => new YearSearchOptionValue(ushort.Parse(value));
    }

    public class DateSearchOptionValue : ISearchOptionValue
    {
        public DateTime Date { get; }
        public string Text { get; }
        public string Label { get; }

        public DateSearchOptionValue(DateTime date)
        {
            Date = date;
            Text = $"{date:d}";
            Label = date.ToLongDateString();
        }
    }

    public class DateSearchOptionFormat : ISearchOptionFormat
    {
        public bool CanParseValue(string value)
            => DateTime.TryParse(value, out DateTime _);

        public ISearchOptionValue ParseValue(string value)
            => new DateSearchOptionValue(DateTime.Parse(value));
    }


    public class MomentSearchOptionValue : ISearchOptionValue
    {
        private static readonly string[] moments = new string[]
        {
            "dayago", "weekago", "monthago", "yearago"
        };

        public string Text { get; }
        public string Label { get; }

        private MomentSearchOptionValue(string moment)
        {
            Text = moment.ToLower();
            Label = string.Empty;
        }

        public static bool CanParse(string value)
            => moments.Contains(value.ToLower());

        public static ISearchOptionValue Parse(string value)
            => new MomentSearchOptionValue(value);
    }

    public class MomentSearchOptionFormat : ISearchOptionFormat
    {
        public bool CanParseValue(string value)
            => MomentSearchOptionValue.CanParse(value);

        public ISearchOptionValue ParseValue(string value)
            => MomentSearchOptionValue.Parse(value);
    }

    public abstract class DateSearchOptionKey : ISearchOptionKey
    {
        public abstract string Text { get; }
        public abstract string Label { get; }

        public ISearchOptionFormat Format { get; } = new SearchOptionFormatCollection
        {
            new YearSearchOptionFormat(),
            new DateSearchOptionFormat(),
            new MomentSearchOptionFormat(),
        };

        public string[] SuggestionValues
        {
            get
            {
                var now = DateTime.Now;

                return new string[] {
                    "dayago", "weekago", "monthago", "yearago", // moments
                    (now.Year - 2).ToString(), (now.Year - 5).ToString(), // years
                    $"{now:M}", $"{now:dd/MM/yyyy}", // date

                };
            }
        }

        public abstract string ProvideFilter(ISearchOptionValue value);

    }
    public class BeforeSearchOptionKey : DateSearchOptionKey
    {
        public override string Text => "before";
        public override string Label => "Before a date";

        public override string ProvideFilter(ISearchOptionValue value)
        {
            if (value is YearSearchOptionValue year)
            {
                return $"System.ItemDate:<={year.Year}";
            }
            if (value is DateSearchOptionValue date)
            {
                return string.Empty;
            }
            if (value is MomentSearchOptionValue moment)
            {
                return string.Empty;
            }
            return string.Empty;
        }
    }
    public class AfterSearchOptionKey : DateSearchOptionKey
    {
        public override string Text => "after";
        public override string Label => "After a date";

        public override string ProvideFilter(ISearchOptionValue value)
        {
            if (value is YearSearchOptionValue year)
            {
                return $"System.ItemDate:>={year.Year}";
            }
            if (value is DateSearchOptionValue date)
            {
                return string.Empty;
            }
            if (value is MomentSearchOptionValue moment)
            {
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
