using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Date : IEquatable<Date>, IComparable<Date>, IFormattable
    {
        private readonly DateTime date;

        public static Date Today => new(DateTime.Today);

        public static Date MinValue { get; } = new Date(1600, 1, 1);
        public static Date MaxValue { get; } = new Date(9999, 12, 31);

        public ushort Year => (ushort)date.Year;
        public ushort Month => (ushort)date.Month;
        public ushort Day => (ushort)date.Day;

        public Date(ushort year, ushort month, ushort day) : this(new DateTime(year, month, day)) { }
        public Date(DateTime date) => this.date = date.Date;

        public static bool operator ==(Date d1, Date d2) => d1.date == d2.date;
        public static bool operator !=(Date d1, Date d2) => d1.date != d2.date;
        public static bool operator <(Date d1, Date d2) => d1.date < d2.date;
        public static bool operator >(Date d1, Date d2) => d1.date > d2.date;
        public static bool operator <=(Date d1, Date d2) => d1.date <= d2.date;
        public static bool operator >=(Date d1, Date d2) => d1.date >= d2.date;

        public override int GetHashCode() => date.GetHashCode();
        public override bool Equals(object other) => other is Date date && Equals(date);
        public bool Equals(Date other) => other.date.Equals(date);
        public int CompareTo(Date other) => other.date.CompareTo(date);

        public override string ToString() => date.ToString("D");
        public string ToString(string format) => date.ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider) => date.ToString(format, formatProvider);

        public Date AddDays(int days) => new(date.AddDays(days));
        public Date AddMonths(int months) => new(date.AddMonths(months));
        public Date AddYears(int years) => new(date.AddYears(years));
    }

    public class DateRange : IEquatable<DateRange>, IFormattable
    {
        private readonly Range range;

        public Date MinDate => range.MinDate;
        public Date MaxDate => range.MaxDate;

        public DateRange() : this(new AlwaysRange()) { }
        public DateRange(Date minDate, Date maxDate) => range = GetRange(minDate, maxDate);
        private DateRange(Range range) => this.range = range;

        public void Deconstruct(out Date minDate, out Date maxDate) => (minDate, maxDate) = (MinDate, MaxDate);

        public override int GetHashCode() => range.GetHashCode();
        public override bool Equals(object other) => range.Equals(other);
        public virtual bool Equals(DateRange other) => range.Equals(other.range);

        public override string ToString() => range.ToString();
        public string ToString(string format) => range.ToString(format);
        public virtual string ToString(string format, IFormatProvider formatProvider) => range.ToString(format, formatProvider);

        public static IEnumerable<DateRange> EnumerateNamedRanges() => EnumerateNamedRanges(Date.Today);
        public static IEnumerable<DateRange> EnumerateNamedRanges (Date today)
        {
            yield return new (new AlwaysRange());
            yield return new (new TodayRange(today));
            yield return new (new YesterdayRange(today));
            yield return new (new ThisWeekRange(today));
            yield return new (new LastWeekRange(today));
            yield return new (new ThisMonthRange(today));
            yield return new (new LastMonthRange(today));
            yield return new (new ThisYearRange(today));
            yield return new (new LastYearRange(today));
        }

        private static Range GetRange(Date minDate, Date maxDate)
        {
            if (minDate > maxDate)
            {
                (minDate, maxDate) = (maxDate, minDate);
            }

            var named = EnumerateNamedRanges().FirstOrDefault(range => (range.MinDate, range.MaxDate).Equals((minDate, maxDate)));
            if (named is not null)
            {
                return named.range;
            }

            if (maxDate.Equals(Date.MinValue))
            {
                return new MinRange();
            }
            if (minDate.Equals(Date.MaxValue))
            {
                return new MaxRange();
            }
            if (minDate.Equals(maxDate))
            {
                return new EqualRange(minDate);
            }
            if (minDate.Equals(Date.MinValue))
            {
                return new BeforeRange(maxDate);
            }
            if (maxDate.Equals(Date.MaxValue))
            {
                return new AfterRange(minDate);
            }
            return new BetweenRange(minDate, maxDate);
        }

        private abstract class Range
        {
            public virtual Date MinDate { get; } = Date.MinValue;
            public virtual Date MaxDate { get; } = Date.MaxValue;

            public virtual string Name => null;
            public virtual string ShortFormat => string.Empty;
            public virtual string FullFormat => ShortFormat;

            public void Deconstruct(out Date minDate, out Date maxDate)
                => (minDate, maxDate) = (MinDate, MaxDate);

            public override int GetHashCode() => (MinDate, MaxDate).GetHashCode();
            public override bool Equals(object other) => other is Range range && Equals(range);
            public bool Equals(Range other) => (other.MinDate, other.MaxDate).Equals((MinDate, MaxDate));

            public override string ToString() => ToString("G");
            public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
            public string ToString(string format, IFormatProvider formatProvider) => format switch
            {
                "G" => ToString("N", formatProvider),
                "n" => Name ?? ToString("r", formatProvider),
                "N" => Name ?? ToString("R", formatProvider),
                "r" => string.Format(ShortFormat, $"{MinDate:d}", $"{MaxDate:d}"),
                "R" => string.Format(FullFormat, $"{MinDate:d}", $"{MaxDate:d}"),
                _ => string.Empty,
            };
        }
        private class AlwaysRange : Range
        {
            public override string Name => string.Empty;
        }
        private class TodayRange : EqualRange
        {
            public override string Name => "ItemTimeText_Today".GetLocalized();
            public TodayRange(Date today) : base(today) {}
        }
        private class YesterdayRange : EqualRange
        {
            public override string Name => "ItemTimeText_Yesterday".GetLocalized();
            public YesterdayRange(Date today) : base(today.AddDays(-1)) {}
        }
        private class ThisWeekRange : BetweenRange
        {
            public override string Name => "ItemDateText_ThisWeek".GetLocalized();
            public ThisWeekRange(Date today) : base(today.AddDays(-6), today) {}
        }
        private class LastWeekRange : BetweenRange
        {
            public override string Name => "ItemDateText_LastWeek".GetLocalized();
            public LastWeekRange(Date today) : base(today.AddYears(-13), today.AddYears(-7)) {}
        }
        private class ThisMonthRange : BetweenRange
        {
            public override string Name => "ItemDateText_ThisMonth".GetLocalized();
            public ThisMonthRange(Date today) : base(today.AddMonths(-1).AddDays(1), today) {}
        }
        private class LastMonthRange : BetweenRange
        {
            public override string Name => "ItemDateText_LastMonth".GetLocalized();
            public LastMonthRange(Date today) : base(today.AddMonths(-2).AddDays(1), today.AddMonths(-1)) {}
        }
        private class ThisYearRange : BetweenRange
        {
            public override string Name => "ItemDateText_ThisYear".GetLocalized();
            public ThisYearRange(Date today) : base(today.AddYears(-1).AddDays(1), today) {}
        }
        private class LastYearRange : BetweenRange
        {
            public override string Name => "ItemDateText_Older".GetLocalized();
            public LastYearRange(Date today) : base(today.AddYears(-2).AddDays(1), today.AddYears(-1)) {}
        }
        private class MinRange : Range
        {
            public override Date MaxDate { get; } = Date.MinValue;
        }
        private class MaxRange : Range
        {
            public override Date MinDate { get; } = Date.MaxValue;
        }
        private class EqualRange : Range
        {
            public override Date MinDate { get; }
            public override Date MaxDate => MinDate;
            public override string ShortFormat => "{1}";
            public EqualRange(Date date) => MinDate = date;
        }
        private class BeforeRange : Range
        {
            public override Date MaxDate { get; }
            public override string ShortFormat => "< {1}";
            public override string FullFormat => "Before {1}";
            public BeforeRange(Date maxDate) => MaxDate = maxDate;
        }
        private class AfterRange : Range
        {
            public override Date MinDate { get; }
            public override string ShortFormat => "> {0}";
            public override string FullFormat => "After {0}";
            public AfterRange(Date minDate) => MinDate = minDate;
        }
        private class BetweenRange : Range
        {
            public override Date MinDate { get; }
            public override Date MaxDate { get; }
            public override string ShortFormat => "{0} - {1}";
            public override string FullFormat => "Between {0} and {1}";
            public BetweenRange(Date minDate, Date maxDate) => (MinDate, MaxDate) = (minDate, maxDate);
        }
    }
}
