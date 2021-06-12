using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text { get; } = "date";
        public virtual string Label { get; } = "Date of creation";

        public ISearchOptionFormat Format { get; } = new PeriodSearchOptionFormat();

        public string[] SuggestionValues { get; } = new string[]
        {
            "today", "yesterday", "thisweek", "thismonth", "thisyear",
            "<=2019", "02/03/2017..05/03/2017", "2018", "04/05/2019"
        };

        public string ProvideFilter(ISearchOptionValue value)
        {
            if (value is PeriodSearchOptionValue period)
            {
                return ProvideFilter(period);
            }
            throw new ArgumentException();
        }
        public virtual string ProvideFilter(PeriodSearchOptionValue period)
            => $"System.ItemDate:{period.ToAdvancedQuerySyntax()}";
    }
    public class ModificationDateSearchOptionKey : DateSearchOptionKey
    {
        public override string Text { get; } = "modification";
        public override string Label { get; } = "Date of last modification";

        public override string ProvideFilter(PeriodSearchOptionValue period)
            => $"System.DateModified:{period.ToAdvancedQuerySyntax()}";
    }

    public interface IPeriodSearchOptionValue : ISearchOptionValue
    {
        DateTime MinDate { get; }
        DateTime MaxDate { get; }
    }

    public class PeriodSearchOptionFormat : ISearchOptionFormat
    {
        private readonly ISearchOptionFormat format = new SearchOptionFormatCollection
        {
            new MomentSearchOptionFormat(),
            new YearSearchOptionFormat(),
            new DateSearchOptionFormat(),
        };

        public bool CanParseValue(string value)
        {
            if (value.StartsWith("<=") || value.StartsWith(">=") || value.StartsWith(".."))
            {
                return format.CanParseValue(value.Substring(2));
            }
            if (value.StartsWith("<") || value.StartsWith(">"))
            {
                return format.CanParseValue(value.Substring(1));
            }
            if (value.EndsWith(".."))
            {
                return format.CanParseValue(value.Substring(0, value.Length - 2));
            }
            if (value.Contains(".."))
            {
                var parts = value.Split("..", 2);
                return format.CanParseValue(parts[0]) && format.CanParseValue(parts[1]);
            }
            return format.CanParseValue(value);
        }
        public ISearchOptionValue ParseValue(string value)
        {
            if (value.StartsWith("<="))
            {
                return new PeriodSearchOptionValue(ValueRelation.LessOrEqual, null, Parse(value.Substring(2)));
            };
            if (value.StartsWith(">="))
            {
                return new PeriodSearchOptionValue(ValueRelation.GreaterOrEqual, Parse(value.Substring(2)), null);
            }
            if (value.StartsWith("<"))
            {
                return new PeriodSearchOptionValue(ValueRelation.Less, null, Parse(value.Substring(1)));
            }
            if (value.StartsWith(">"))
            {
                return new PeriodSearchOptionValue(ValueRelation.Greater, Parse(value.Substring(1)), null);
            }
            if (value.StartsWith(".."))
            {
                return new PeriodSearchOptionValue(ValueRelation.LessOrEqual, null, Parse(value.Substring(2)));
            }
            if (value.EndsWith(".."))
            {
                return new PeriodSearchOptionValue(ValueRelation.GreaterOrEqual, Parse(value.Substring(2)), null);
            }
            if (value.Contains(".."))
            {
                var parts = value.Split("..", 2);
                return new PeriodSearchOptionValue(ValueRelation.Between, Parse(parts[0]), Parse(parts[1]));
            }
            var parsed = Parse(value);
            return new PeriodSearchOptionValue(ValueRelation.Equal, parsed, parsed);

            IPeriodSearchOptionValue Parse(string value) => format.ParseValue(value) as IPeriodSearchOptionValue;
        }
    }

    public class PeriodSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public ValueRelation Relation { get; }

        public DateTime MinDate { get; }
        public DateTime MaxDate { get; }

        public PeriodSearchOptionValue(ValueRelation relation, IPeriodSearchOptionValue minValue, IPeriodSearchOptionValue maxValue)
        {
            Relation = relation;

            if (!(minValue is null))
            {
                MinDate = minValue.MinDate;
            }
            if (!(maxValue is null))
            {
                MaxDate = maxValue.MaxDate;
            }

            Text = relation switch
            {
                ValueRelation.Equal => minValue.Text,
                ValueRelation.Less => $"<{maxValue.Text}",
                ValueRelation.Greater => $">{minValue.Text}",
                ValueRelation.LessOrEqual => $"<={maxValue.Text}",
                ValueRelation.GreaterOrEqual => $">={minValue.Text}",
                ValueRelation.Between => $"{minValue.Text}..{maxValue.Text}",
                _ => throw new ArgumentException()
            };
            Label = relation switch
            {
                ValueRelation.Equal => minValue.Label,
                ValueRelation.Less => $"< {maxValue.Label}",
                ValueRelation.Greater => $"> {minValue.Label}",
                ValueRelation.LessOrEqual => $"<= {maxValue.Label}",
                ValueRelation.GreaterOrEqual => $">= {minValue.Label}",
                ValueRelation.Between => $"{minValue.Label} -> {maxValue.Label}",
                _ => throw new ArgumentException()
            };
        }

        public string ToAdvancedQuerySyntax() => Relation switch
        {
            ValueRelation.Equal => $"{MinDate:yyyy-MM-dd}..{MaxDate:yyyy-MM-dd}",
            ValueRelation.Less => $"<{MaxDate:yyyy-MM-dd}",
            ValueRelation.Greater => $">{MinDate:yyyy-MM-dd}",
            ValueRelation.LessOrEqual => $"<={MaxDate:yyyy-MM-dd}",
            ValueRelation.GreaterOrEqual => $">={MinDate:yyyy-MM-dd}",
            ValueRelation.Between => $"{MinDate:yyyy-MM-dd}..{MaxDate:yyyy-MM-dd}",
            _ => throw new Exception()
        };
    }

    public class DateSearchOptionFormat : ISearchOptionFormat
    {
        public bool CanParseValue(string value)
            => DateTime.TryParse(value, out DateTime _);

        public ISearchOptionValue ParseValue(string value)
            => new DateSearchOptionValue(DateTime.Parse(value));

    }
    public class DateSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public DateTime Date { get; }

        public DateTime MinDate => Date;
        public DateTime MaxDate => Date;

        public DateSearchOptionValue(DateTime date)
        {
            Text = $"{date.Date:d}";
            Label = $"{date.Date:D}";
            Date = date.Date;
        }
    }

    public class YearSearchOptionFormat : ISearchOptionFormat
    {
        public bool CanParseValue(string value)
            => ushort.TryParse(value, out ushort year) && year >= 1900 && year <= 9999;

        public ISearchOptionValue ParseValue(string value)
            => new YearSearchOptionValue(ushort.Parse(value));

    }
    public class YearSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public ushort Year { get; }

        public DateTime MinDate => new DateTime(Year, 1, 1);
        public DateTime MaxDate => new DateTime(Year, 12, 31);

        public YearSearchOptionValue(ushort year)
        {
            Text = $"{year}";
            Label = $"Year {year}";
            Year = year;
        }
    }

    public class MomentSearchOptionFormat : ISearchOptionFormat
    {
        private readonly string[] moments = new string[] { "today", "yesterday", "thisweek", "thismonth", "thisyear" };

        public bool CanParseValue(string moment) => moments.Contains(moment.ToLower());

        public ISearchOptionValue ParseValue(string moment) => new MomentSearchOptionValue(moment);
    }
    public class MomentSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public DateTime MinDate { get; }
        public DateTime MaxDate { get; }

        public MomentSearchOptionValue(string moment)
        {
            moment = moment.ToLower();
            var today = DateTime.Now.Date;

            Text = moment switch
            {
                "today" => "today",
                "yesterday" => "yesterday",
                "thisweek" => "thisWeek",
                "thismonth" => "thisMonth",
                "thisyear" => "thisYear",
                _ => throw new ArgumentException()
            };
            Label = moment switch
            {
                "today" => "Today",
                "yesterday" => "Yesterday",
                "thisweek" => "This week",
                "thismonth" => "This month",
                "thisyear" => "This year",
                _ => throw new ArgumentException()
            };
            MinDate = moment switch
            {
                "today" => today,
                "yesterday" => today.AddDays(-1),
                "thisweek" => today.AddDays(-7),
                "thismonth" => today.AddMonths(-1),
                "thisyear" => today.AddYears(-1),
                _ => throw new ArgumentException()
            };
            MaxDate = today;
        }
    }
}
