using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Date : IEquatable<Date>, IComparable<Date>, IFormattable
    {
        private const ushort minYear = 1600;
        private const ushort maxYear = 9999;

        private readonly DateTime date;

        public static readonly Date MinValue = new(minYear, 1, 1);
        public static readonly Date MaxValue = new(maxYear, 12, 31);

        public static Date Today => new(DateTime.Today);

        public ushort Year => (ushort)date.Year;
        public ushort Month => (ushort)date.Month;
        public ushort Day => (ushort)date.Day;

        public DateTime DateTime => date;
        public DateTimeOffset Offset => new(date);

        public Date(ushort year, ushort month, ushort day)
            : this(new DateTime(year, month, day)) {}
        public Date(DateTime date) => this.date = date.Year switch
        {
            < minYear => MinValue.date,
            > maxYear => MaxValue.date,
            _ => date.Date,
        };

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

        public override string ToString() => date.ToString("d");
        public string ToString(string format) => date.ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format is null || format == "G")
            {
                return date.ToString("d", formatProvider);
            }
            return date.ToString(format, formatProvider);
        }

        public Date AddDays(int days) => new(date.AddDays(days));
        public Date AddMonths(int months) => new(date.AddMonths(months));
        public Date AddYears(int years) => new(date.AddYears(years));
    }

    public struct DateRange : IRange<Date>, IEquatable<DateRange>, IFormattable
    {
        public static event EventHandler TodayUpdated;

        private static DateRange always;
        private static DateRange today;
        private static DateRange yesterday;
        private static DateRange thisWeek;
        private static DateRange lastWeek;
        private static DateRange thisMonth;
        private static DateRange lastMonth;
        private static DateRange thisYear;
        private static DateRange older;

        public static DateRange None => new(false);

        public static DateRange Always => always;
        public static DateRange Today => today;
        public static DateRange Yesterday => yesterday;
        public static DateRange ThisWeek => thisWeek;
        public static DateRange LastWeek => lastWeek;
        public static DateRange ThisMonth => thisMonth;
        public static DateRange LastMonth => lastMonth;
        public static DateRange ThisYear => thisYear;
        public static DateRange Older => older;

        public bool IsNamed => GetIsNamed();

        public Date MinValue { get; }
        public Date MaxValue { get; }

        static DateRange() => UpdateToday();

        public DateRange(Date minValue, Date maxValue)
        {
            var today = Date.Today;

            minValue = minValue < today ? minValue : today;
            maxValue = maxValue < today ? maxValue : today;

            (MinValue, MaxValue) = (minValue <= maxValue) ? (minValue, maxValue) : (maxValue, minValue);
        }

        public DateRange(Date minValue, DateRange maxRange)
            : this(Min(minValue, maxRange.MinValue), Max(minValue, maxRange.MaxValue)) {}
        public DateRange(DateRange minRange, Date maxValue)
            : this(Min(minRange.MinValue, maxValue), Max(minRange.MaxValue, maxValue)) {}
        public DateRange(DateRange minRange, DateRange maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        private DateRange(bool _) => (MinValue, MaxValue) = (Date.MaxValue, Date.MinValue);

        public void Deconstruct(out Date minValue, out Date maxValue)
            => (minValue, maxValue) = (MinValue, MaxValue);
        public void Deconstruct(out bool isNamed, out Date minValue, out Date maxValue)
            => (isNamed, minValue, maxValue) = (IsNamed, MinValue, MaxValue);

        public static void UpdateToday()
        {
            var date = Date.Today;

            always = new(Date.MinValue, date);
            today = new(date, date);
            yesterday = new(date.AddDays(-1), date.AddDays(-1));
            thisWeek = new(date.AddDays(-6), date.AddDays(-2));
            lastWeek = new(date.AddDays(-13), date.AddDays(-7));
            thisMonth = new(date.AddMonths(-1).AddDays(1), date.AddDays(-14));
            lastMonth = new(date.AddMonths(-2).AddDays(1), date.AddMonths(-1));
            thisYear = new(date.AddYears(-1).AddDays(1), date.AddMonths(-2));
            older = new(Date.MinValue, date.AddYears(-1));

            TodayUpdated?.Invoke(null, EventArgs.Empty);
        }

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is DateRange range && Equals(range);
        public bool Equals(DateRange other)
            => other is DateRange range && (range.MinValue, range.MinValue) == (MinValue, MaxValue);

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Equals(None) || Equals(Always))
            {
                return string.Empty;
            }

            if (format == "g")
            {
                return ToString("n", formatProvider);
            }
            if (format is null || format == "G")
            {
                return ToString("N", formatProvider);
            }

            var (isNamed, minValue, maxValue) = this;
            bool useName = isNamed && format.ToLower() == "n";

            bool hasMin = minValue > Date.MinValue;
            bool hasMax = maxValue < Date.Today;

            string minLabel = GetMinLabel();
            string maxLabel = GetMaxLabel();

            return format switch
            {
                "n" => string.Format(GetShortFormat(), minLabel, maxLabel),
                "N" => string.Format(GetFullFormat(), minLabel, maxLabel),
                "r" => string.Format(GetShortFormat(), minLabel, maxLabel),
                "R" => string.Format(GetFullFormat(), minLabel, maxLabel),
                "q" => string.Format(GetQueryFormat(), minValue, maxValue),
                "Q" => string.Format(GetQueryFormat(), minValue, maxValue),
                _ => string.Empty,
            };

            string GetMinLabel() => useName switch
            {
                true when Today.MinValue.Equals(minValue) => "ItemTimeText_Today".GetLocalized(),
                true when Yesterday.MinValue.Equals(minValue) => "ItemTimeText_Yesterday".GetLocalized(),
                true when ThisWeek.MinValue.Equals(minValue) => "ItemTimeText_ThisWeek".GetLocalized(),
                true when LastWeek.MinValue.Equals(minValue) => "ItemTimeText_LastWeek".GetLocalized(),
                true when ThisMonth.MinValue.Equals(minValue) => "ItemTimeText_ThisMonth".GetLocalized(),
                true when LastMonth.MinValue.Equals(minValue) => "ItemTimeText_LastMonth".GetLocalized(),
                true when ThisYear.MinValue.Equals(minValue) => "ItemTimeText_ThisYear".GetLocalized(),
                true when Older.MinValue.Equals(minValue) => "ItemTimeText_Older".GetLocalized(),
                true => string.Empty,
                false => $"{minValue}",
            };
            string GetMaxLabel() => useName switch
            {
                true when Today.MaxValue.Equals(maxValue) => "ItemTimeText_Today".GetLocalized(),
                true when Yesterday.MaxValue.Equals(maxValue) => "ItemTimeText_Yesterday".GetLocalized(),
                true when ThisWeek.MaxValue.Equals(maxValue) => "ItemTimeText_ThisWeek".GetLocalized(),
                true when LastWeek.MaxValue.Equals(maxValue) => "ItemTimeText_LastWeek".GetLocalized(),
                true when ThisMonth.MaxValue.Equals(maxValue) => "ItemTimeText_ThisMonth".GetLocalized(),
                true when LastMonth.MaxValue.Equals(maxValue) => "ItemTimeText_LastMonth".GetLocalized(),
                true when ThisYear.MaxValue.Equals(maxValue) => "ItemTimeText_ThisYear".GetLocalized(),
                true when Older.MaxValue.Equals(maxValue) => "ItemTimeText_Older".GetLocalized(),
                true => string.Empty,
                false => $"{maxValue}",
            };

            string GetShortFormat() => (hasMin, hasMax) switch
            {
                _ when minLabel == maxLabel => "{0}",
                (false, _) => "< {1}",
                (_, false) => "> {0}",
                _ => "{0} - {1}",
            };
            string GetFullFormat() => (hasMin, hasMax) switch
            {
                _ when minLabel == maxLabel => "{0}",
                (false, _) => "SearchDateRange_Before".GetLocalized(),
                (_, false) => "SearchDateRange_After".GetLocalized(),
                _ => "SearchDateRange_Between".GetLocalized(),
            };
            string GetQueryFormat() => (hasMin, hasMax) switch
            {
                _ when minValue == maxValue => "{0::yyyyMMdd}",
                (false, _) => "<{1:yyyyMMdd}",
                (_, false) => ">{0:yyyyMMdd}",
                _ => "{0:yyyyMMdd}..{1:yyyyMMdd}",
            };
        }

        public static DateRange operator +(DateRange a, DateRange b) => new(a, b);
        public static DateRange operator -(DateRange a, DateRange b) => Substract(a, b);
        public static bool operator ==(DateRange a, DateRange b) => a.Equals(b);
        public static bool operator !=(DateRange a, DateRange b) => !a.Equals(b);
        public static bool operator <(DateRange a, DateRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(DateRange a, DateRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(DateRange a, DateRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(DateRange a, DateRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Date date) => MinValue <= date && date <= MaxValue;
        public bool Contains(DateRange range) => MinValue <= range.MinValue && range.MaxValue <= MaxValue;

        private static Date Min(Date a, Date b) => a <= b ? a : b;
        private static Date Max(Date a, Date b) => a >= b ? a : b;

        private static DateRange Substract(DateRange a, DateRange b)
        {
            if (b.MinValue == a.MinValue && b.MaxValue < a.MaxValue)
            {
                return new(b.MaxValue.AddDays(1), a.MaxValue);
            }
            if (b.MaxValue == a.MaxValue && b.MinValue > a.MinValue)
            {
                return new(a.MinValue, b.MinValue.AddDays(-1));
            }
            return None;
        }

        private bool GetIsNamed()
        {
            var (minValue, maxValue) = this;
            var named = new List<DateRange> { Today, Yesterday, ThisWeek, LastWeek, ThisMonth, LastMonth, ThisYear, Older };
            return named.Any(n => n.MinValue == minValue) && named.Any(n => n.MaxValue == maxValue);
        }
    }
}
