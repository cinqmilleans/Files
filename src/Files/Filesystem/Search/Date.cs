using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Date : IEquatable<Date>, IComparable<Date>, IFormattable
    {
        public static event EventHandler<TodayUpdatedEventArgs> TodayUpdated;

        private const ushort minYear = 1600;
        private const ushort maxYear = 9999;

        private readonly DateTime date;

        public static readonly Date MinValue = new(minYear, 1, 1);
        public static readonly Date MaxValue = new(maxYear, 12, 31);

        private static Date today = new(DateTime.Today);
        public static Date Today => today;

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

        public static void UpdateToday()
        {
            var oldToday = Today;
            var newToday = new Date(DateTime.Today);

            if (oldToday < newToday)
            {
                today = new(DateTime.Today);
                TodayUpdated.Invoke(null, new TodayUpdatedEventArgs(oldToday, newToday));
            }
        }

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

        public bool IsNamed
        {
            get
            {
                var (minValue, maxValue) = (MinValue, MaxValue);
                var named = new List<DateRange> { Today, Yesterday, ThisWeek, LastWeek, ThisMonth, LastMonth, ThisYear, Older };
                return named.Any(n => n.MinValue == minValue) && named.Any(n => n.MaxValue == maxValue);
            }
        }

        public RangeDirections Direction
        {
            get
            {
                if (Equals(None) || Equals(Always))
                {
                    return RangeDirections.None;
                }

                bool hasMin = MinValue > Date.MinValue;
                bool hasMax = MaxValue < Date.Today;

                return (hasMin, hasMax) switch
                {
                    (false, false) => RangeDirections.None,
                    (true, false) => RangeDirections.GreaterThan,
                    (false, true) => RangeDirections.LessThan,
                    _ when MinValue == MaxValue => RangeDirections.EqualTo,
                    _ => RangeDirections.Between,
                };
            }
        }

        public Date MinValue { get; }
        public Date MaxValue { get; }

        public IRange<string> Label => ToLabel();

        static DateRange()
        {
            Date.TodayUpdated += Date_TodayUpdated;
            UpdateToday();
        }
        public DateRange(Date minValue, Date maxValue)
        {
            Date today = Date.Today;

            minValue = minValue < today ? minValue : today;
            maxValue = maxValue < today ? maxValue : today;

            (MinValue, MaxValue) = minValue < maxValue ? (minValue, maxValue) : (maxValue, minValue);
        }

        public DateRange(Date minValue, DateRange maxRange)
            : this(Min(minValue, maxRange.MinValue), Max(minValue, maxRange.MaxValue)) {}
        public DateRange(DateRange minRange, Date maxValue)
            : this(Min(minRange.MinValue, maxValue), Max(minRange.MaxValue, maxValue)) {}
        public DateRange(DateRange minRange, DateRange maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        public DateRange(bool _) => (MinValue, MaxValue) = (Date.MaxValue, Date.MinValue);

        public void Deconstruct(out bool isNamed, out RangeDirections direction, out Date minValue, out Date maxValue)
            => (isNamed, direction, minValue, maxValue) = (IsNamed, Direction, MinValue, MaxValue);

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is DateRange range && Equals(range);
        public bool Equals(DateRange other)
            => other is DateRange range && range.MinValue == MinValue && range.MaxValue == MaxValue;

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Direction == RangeDirections.None)
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

            bool useName = IsNamed && format.ToLower() == "n";
            var (direction, minLabel, maxLabel) = ToLabel(useName);

            string text =  format switch
            {
                "n" => GetShortFormat(),
                "N" => GetFullFormat(),
                "r" => GetShortFormat(),
                "R" => GetFullFormat(),
                "q" => GetQueryFormat(),
                "Q" => GetQueryFormat(),
                _ => string.Empty,
            };

            return string.Format(text, minLabel, maxLabel);

            string GetShortFormat() => direction switch
            {
                RangeDirections.EqualTo => "{0}",
                RangeDirections.LessThan => "< {1}",
                RangeDirections.GreaterThan => "> {0}",
                _ => "{0} - {1}",
            };
            string GetFullFormat() => direction switch
            {
                RangeDirections.EqualTo => "{0}",
                RangeDirections.LessThan => "SearchDateRange_Before".GetLocalized(),
                RangeDirections.GreaterThan => "SearchDateRange_After".GetLocalized(),
                _ => "SearchDateRange_Between".GetLocalized(),
            };
            string GetQueryFormat() => direction switch
            {
                RangeDirections.EqualTo => "{0::yyyyMMdd}",
                RangeDirections.LessThan => "<{1:yyyyMMdd}",
                RangeDirections.GreaterThan => ">{0:yyyyMMdd}",
                _ => "{0:yyyyMMdd}..{1:yyyyMMdd}",
            };
        }

        public RangeLabel ToLabel(bool useName = true)
        {
            if (Equals(LastWeek))
            {
            }

            useName &= IsNamed;

            if (Equals(None) || Equals(Always))
            {
                return RangeLabel.None;
            }
            if (useName && Equals(Today))
            {
                return new RangeLabel("ItemTimeText_Today".GetLocalized());
            }
            if (useName && Equals(Older))
            {
                return new RangeLabel("ItemTimeText_Older".GetLocalized());
            }

            string minLabel = useName switch
            {
                _ when Direction == RangeDirections.LessThan => string.Empty,
                true when Today.MinValue.Equals(MinValue) => "ItemTimeText_Today".GetLocalized(),
                true when Yesterday.MinValue.Equals(MinValue) => "ItemTimeText_Yesterday".GetLocalized(),
                true when ThisWeek.MinValue.Equals(MinValue) => "ItemTimeText_ThisWeek".GetLocalized(),
                true when LastWeek.MinValue.Equals(MinValue) => "ItemTimeText_LastWeek".GetLocalized(),
                true when ThisMonth.MinValue.Equals(MinValue) => "ItemTimeText_ThisMonth".GetLocalized(),
                true when LastMonth.MinValue.Equals(MinValue) => "ItemTimeText_LastMonth".GetLocalized(),
                true when ThisYear.MinValue.Equals(MinValue) => "ItemTimeText_ThisYear".GetLocalized(),
                false => $"{MinValue}",
                _ => string.Empty,
            };
            string maxLabel = useName switch
            {
                _ when Direction == RangeDirections.GreaterThan => string.Empty,
                true when Yesterday.MaxValue.Equals(MaxValue) => "ItemTimeText_Yesterday".GetLocalized(),
                true when ThisWeek.MaxValue.Equals(MaxValue) => "ItemTimeText_ThisWeek".GetLocalized(),
                true when LastWeek.MaxValue.Equals(MaxValue) => "ItemTimeText_LastWeek".GetLocalized(),
                true when ThisMonth.MaxValue.Equals(MaxValue) => "ItemTimeText_ThisMonth".GetLocalized(),
                true when LastMonth.MaxValue.Equals(MaxValue) => "ItemTimeText_LastMonth".GetLocalized(),
                true when ThisYear.MaxValue.Equals(MaxValue) => "ItemTimeText_ThisYear".GetLocalized(),
                true when Older.MaxValue.Equals(MaxValue) => "ItemTimeText_Older".GetLocalized(),
                false => $"{MaxValue}",
                _ => string.Empty,
            };

            return new RangeLabel(minLabel, maxLabel);
        }

        public static DateRange operator +(DateRange a, DateRange b) => new(a, b);
        public static DateRange operator -(DateRange a, DateRange b) => Substract(a, b);
        public static bool operator ==(DateRange a, DateRange b) => a.Equals(b);
        public static bool operator !=(DateRange a, DateRange b) => !a.Equals(b);
        public static bool operator <(DateRange a, DateRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(DateRange a, DateRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(DateRange a, DateRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(DateRange a, DateRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Date size) => size >= MinValue && size <= MaxValue;
        public bool Contains(DateRange range) => range.MinValue >= MinValue && range.MaxValue <= MaxValue;

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

        private static void Date_TodayUpdated(object sender, TodayUpdatedEventArgs e) => UpdateToday();

        private static void UpdateToday()
        {
            Date date = Date.Today;

            always = new(Date.MinValue, date);
            today = new(date, date);
            yesterday = new(date.AddDays(-1), date.AddDays(-1));
            thisWeek = new(date.AddDays(-6), date.AddDays(-2));
            lastWeek = new(date.AddDays(-13), date.AddDays(-7));
            thisMonth = new(date.AddMonths(-1).AddDays(1), date.AddDays(-14));
            lastMonth = new(date.AddMonths(-2).AddDays(1), date.AddMonths(-1));
            thisYear = new(date.AddYears(-1).AddDays(1), date.AddMonths(-2));
            older = new(Date.MinValue, date.AddYears(-1));
        }
    }

    public class TodayUpdatedEventArgs : EventArgs
    {
        public Date OldToday { get; }
        public Date NewToday { get; }

        public TodayUpdatedEventArgs(Date oldToday, Date newToday)
            => (OldToday, NewToday) = (oldToday, newToday);
    }
}
