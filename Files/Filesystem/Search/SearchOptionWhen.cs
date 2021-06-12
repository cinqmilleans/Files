using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text { get; } = "date";
        public virtual string Label { get; } = "Date of creation";

        public ISearchOptionFormat Format { get; } = new PeriodSearchOptionFormat();

        public string[] SuggestionValues { get; }

        public DateSearchOptionKey()
        {
            SuggestionValues = new string[]
            {
                "today", "thisweek", "thismonth", "thisyear",
                "<=2019", "02/03/2017..05/03/2017", "2018", "24/03/2019"
            };
        }

        public string ProvideFilter(ISearchOptionValue value)
        {
            if (value is PeriodSearchOptionValue period)
            {
                return ProvideFilter(period);
            }
            throw new ArgumentException();
        }
        public virtual string ProvideFilter(PeriodSearchOptionValue period)
            => $"System.ItemDate:>={period.ToAdvancedQuerySyntax()}";
    }
    public class ModificationDateSearchOptionKey : DateSearchOptionKey
    {
        public override string Text { get; } = "modification";
        public override string Label { get; } = "Date of last modification";

        public override string ProvideFilter(PeriodSearchOptionValue period)
            => $"System.DateModified:>={period.ToAdvancedQuerySyntax()}";
    }

    public interface IPeriodSearchOptionValue : ISearchOptionValue
    {
        DateTime ToMinDate();
        DateTime ToMaxDate();
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

        public IPeriodSearchOptionValue MinValue { get; }
        public IPeriodSearchOptionValue MaxValue { get; }

        public PeriodSearchOptionValue(ValueRelation relation, IPeriodSearchOptionValue minValue, IPeriodSearchOptionValue maxValue)
        {
            Relation = relation;
            MinValue = minValue;
            MaxValue = maxValue;

            Text = relation switch
            {
                ValueRelation.Equal => MinValue.Text,
                ValueRelation.Less => $"<{MaxValue.Text}",
                ValueRelation.Greater => $">{MinValue.Text}",
                ValueRelation.LessOrEqual => $"<={MaxValue.Text}",
                ValueRelation.GreaterOrEqual => $">={MinValue.Text}",
                ValueRelation.Between => $"{MinValue.Text}..{MaxValue.Text}",
                _ => throw new ArgumentException()
            };
            Label = relation switch
            {
                ValueRelation.Equal => MinValue.Label,
                ValueRelation.Less => $"< {MaxValue.Label}",
                ValueRelation.Greater => $"> {MinValue.Label}",
                ValueRelation.LessOrEqual => $"<= {MaxValue.Label}",
                ValueRelation.GreaterOrEqual => $">= {MinValue.Label}",
                ValueRelation.Between => $"{MinValue.Label} -> {MaxValue.Label}",
                _ => throw new ArgumentException()
            };
        }

        public DateTime ToMinDate() => MinValue.ToMinDate();
        public DateTime ToMaxDate() => MaxValue.ToMaxDate();

        public string ToAdvancedQuerySyntax() => Relation switch
        {
            ValueRelation.Equal => $"{ToMinDate():d}",
            ValueRelation.Less => $"<{ToMaxDate():d}",
            ValueRelation.Greater => $">{ToMinDate():d}",
            ValueRelation.LessOrEqual => $"<={ToMaxDate():d}",
            ValueRelation.GreaterOrEqual => $">={ToMinDate():d}",
            ValueRelation.Between => $"{ToMinDate():d}..{ToMaxDate():d}",
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

        public DateSearchOptionValue(DateTime date)
        {
            Text = $"{date.Date:d}";
            Label = date.Date.ToLongDateString();
            Date = date.Date;
        }

        public DateTime ToMinDate() => Date;
        public DateTime ToMaxDate() => Date;
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

        public YearSearchOptionValue(ushort year)
        {
            Text = $"{year}";
            Label = $"Year {year}";
            Year = year;
        }

        public DateTime ToMinDate() => new DateTime(Year, 1, 1);
        public DateTime ToMaxDate() => new DateTime(Year, 12, 31);
    }

    public class MomentSearchOptionFormat : ISearchOptionFormat
    {
        private readonly string[] moments = new string[] { "today", "thisweek", "thismonth", "thisyear" };

        public bool CanParseValue(string moment) => moments.Contains(moment.ToLower());

        public ISearchOptionValue ParseValue(string moment) => new MomentSearchOptionValue(moment);
    }
    public class MomentSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public string Moment { get; }

        public MomentSearchOptionValue(string moment)
        {
            moment = moment.ToLower();
            Text = moment switch
            {
                "today" => "today",
                "thisweek" => "thisWeek",
                "thismonth" => "thisMonth",
                "thisyear" => "thisYear",
                _ => throw new ArgumentException()
            };
            Label = moment switch
            {
                "today" => "Today",
                "thisweek" => "This week",
                "thismonth" => "This month",
                "thisyear" => "This year",
                _ => throw new ArgumentException()
            };
            Moment = moment.ToLower();
        }

        public DateTime ToMinDate()
        {
            var today = DateTime.Now.Date;
            return Moment switch
            {
                "today" => today,
                "thisweek" => today.AddDays(-7),
                "thismonth" => today.AddMonths(-1),
                "thisyear" => today.AddYears(-1),
                _ => throw new ArgumentException()
            };
        }
        public DateTime ToMaxDate() => DateTime.Now.Date;
    }
}
