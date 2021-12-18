using System;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Date : IEquatable<Date>, IComparable<Date>, IFormattable
    {
        public static event EventHandler<DateTodayUpdatedEventArgs> TodayUpdated;

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
                TodayUpdated.Invoke(null, new DateTodayUpdatedEventArgs(oldToday, newToday));
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

    public class DateTodayUpdatedEventArgs : EventArgs
    {
        public Date OldToday { get; }
        public Date NewToday { get; }

        public DateTodayUpdatedEventArgs(Date oldToday, Date newToday)
            => (OldToday, NewToday) = (oldToday, newToday);
    }

    public struct DateRange : IRange<Date>, IEquatable<DateRange>
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

        public RangeDirections Direction { get; }

        public Date MinValue { get; }
        public Date MaxValue { get; }

        static DateRange()
        {
            UpdateToday(Date.Today);
            Date.TodayUpdated += Date_TodayUpdated;
        }

        public DateRange(Date minValue, Date maxValue)
        {
            minValue = minValue < Date.Today ? minValue : Date.Today;
            maxValue = maxValue < Date.Today ? maxValue : Date.Today;

            if (minValue > maxValue)
            {
                (minValue, maxValue) = (maxValue, minValue);
            }

            bool hasMin = minValue > Date.MinValue;
            bool hasMax = maxValue > Date.Today;

            var direction = (hasMin, hasMax) switch
            {
                (false, false) => RangeDirections.None,
                (true, false) => RangeDirections.GreaterThan,
                (false, true) => RangeDirections.LessThan,
                _ when minValue == maxValue => RangeDirections.EqualTo,
                _ => RangeDirections.Between,
            };

            (Direction, MinValue, MaxValue) = (direction, maxValue, minValue);
        }

        public DateRange(Date minValue, DateRange maxRange)
            : this(Min(minValue, maxRange.MinValue), Max(minValue, maxRange.MaxValue)) {}
        public DateRange(DateRange minRange, Date maxValue)
            : this(Min(minRange.MinValue, maxValue), Max(minRange.MaxValue, maxValue)) {}
        public DateRange(DateRange minRange, DateRange maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        private DateRange(bool _) => (Direction, MinValue, MaxValue) = (RangeDirections.None, Date.MaxValue, Date.MinValue);

        public void Deconstruct(out Date minValue, out Date maxValue)
            => (minValue, maxValue) = (MinValue, MaxValue);
        public void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue)
            => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is DateRange range && Equals(range);
        public bool Equals(DateRange other)
            => other is DateRange range && (range.MinValue, range.MinValue) == (MinValue, MaxValue);

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

        public IRange<string> ToLabel()
        {
            bool hasMin = MinValue > Date.MinValue;
            bool hasMax = MaxValue < Date.Today;

            var minLevel = MinValue switch
            {
                _ when !hasMin => LabelLevels.None,
                _ when MinValue.Month == 1 && MinValue.Day == 1 => LabelLevels.Year,
                _ when MinValue.Day == 1 => LabelLevels.Month,
                _ => LabelLevels.Date,
            };
            var maxLevel = MinValue switch
            {
                _ when !hasMin => LabelLevels.None,
                _ when MinValue.Month == 12 && MinValue.Day == 31 => LabelLevels.Year,
                _ when MinValue.AddDays(1).Day == 1 => LabelLevels.Month,
                _ => LabelLevels.Date,
            };

            var level = new LabelLevels[] { minLevel, maxLevel }.Max();
            string format = level switch
            {
                LabelLevels.None => string.Empty,
                LabelLevels.Year => "yyyy",
                LabelLevels.Month => DateTime.Now.ToString("MMM yyyy"),
                _ => "d",
            };

            var minLabel = hasMin ? MinValue.ToString(format) : string.Empty;
            var maxLabel = hasMax ? MaxValue.ToString(format) : string.Empty;

            return new RangeLabel(minLabel, maxLabel);
        }

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

        private static void Date_TodayUpdated(object sender, DateTodayUpdatedEventArgs e) => UpdateToday(e.NewToday);

        private static void UpdateToday(Date date)
        {
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

        private enum LabelLevels : ushort
        {
            None,
            Year,
            Month,
            Date,
        }
    }
}
