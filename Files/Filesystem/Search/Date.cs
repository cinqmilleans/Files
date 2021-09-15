using Microsoft.Toolkit.Uwp;
using System;
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

        public DateTime dateTime => date;
        public DateTimeOffset offset => new DateTimeOffset(date);

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

        public override string ToString() => date.ToString("d");
        public string ToString(string format) => date.ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider) => date.ToString(format, formatProvider);

        public Date AddDays(int days) => new(date.AddDays(days));
        public Date AddMonths(int months) => new(date.AddMonths(months));
        public Date AddYears(int years) => new(date.AddYears(years));
    }

    public interface IDateRange : IEquatable<IDateRange>, IFormattable
    {
        Date MinDate { get; }
        Date MaxDate { get; }

        string ToString(string format);
    }

    public interface IDateRangeFactory
    {
        IDateRange Build();
        IDateRange Build(NameDateRange.Names name);
        IDateRange Build(NameDateRange.Names minName, NameDateRange.Names maxName);
        IDateRange Build(Date date);
        IDateRange Build(Date minDate, Date maxDate);
    }

    public class DateRangeFactory : IDateRangeFactory
    {
        private readonly Date today;

        public DateRangeFactory() => today = Date.Today;
        public DateRangeFactory(Date today) => this.today = today;

        public IDateRange Build() => NameDateRange.All;
        public IDateRange Build(NameDateRange.Names name) => new NameDateRange(today, name);
        public IDateRange Build(NameDateRange.Names minName, NameDateRange.Names maxName) => new NameDateRange(today, minName, maxName);
        public IDateRange Build(Date date) => Build(date, date);
        public IDateRange Build(Date minDate, Date maxDate)
        {
            if (minDate > today)
            {
                minDate = today;
            }
            if (maxDate > today)
            {
                maxDate = today;
            }
            if (minDate > maxDate)
            {
                (minDate, maxDate) = (maxDate, minDate);
            }

            if (minDate == Date.MinValue || maxDate == today)
            {
                return NameDateRange.All;
            }

            NameDateRange.Names minName = NameDateRange.Names.Older;
            NameDateRange.Names maxName = NameDateRange.Names.Today;

            var names = Enum.GetValues(typeof(NameDateRange.Names)).Cast<NameDateRange.Names>();
            foreach (var name in names)
            {
                var range = new NameDateRange(today, name, name);
                if (range.MinDate == minDate)
                {
                    minName = range.MinName;
                }
                if (range.MaxDate == maxDate)
                {
                    maxName = range.MaxName;
                }
            }

            if (minName != NameDateRange.Names.Older && maxName != NameDateRange.Names.Today)
            {
                return new NameDateRange(today, minName, maxName);
            }
            return new DateRange(today, minDate, maxDate);
        }
    }

    public struct DateRange : IDateRange
    {
        private readonly Date today;

        public Date MinDate { get; }
        public Date MaxDate { get; }

        public DateRange(Date minDate, Date maxDate) => (today, MinDate, MaxDate) = (Date.Today, minDate, maxDate);
        public DateRange(Date today, Date minDate, Date maxDate) => (this.today, MinDate, MaxDate) = (today, minDate, maxDate);

        public void Deconstruct(out Date minDate, out Date maxDate) => (minDate, maxDate) = (MinDate, MaxDate);

        public override int GetHashCode() => (MinDate, MaxDate).GetHashCode();
        public override bool Equals(object other) => other is IDateRange range && Equals(range);
        public bool Equals(IDateRange other) => other is DateRange range && range.MinDate == MinDate && range.MaxDate == MaxDate;

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "n")
            {
                return ToString("r", formatProvider);
            }
            if (format == "N")
            {
                return ToString("R", formatProvider);
            }

            if (MinDate == MaxDate)
            {
                return $"{MinDate:d}";
            }

            bool hasMin = MinDate > Date.MinValue;
            bool hasMax = MaxDate < today;

            return string.Format(GetFormat(), $"{MinDate:d}", $"{MaxDate:d}");

            string GetFormat() => (format, hasMin, hasMax) switch
            {
                (_, false, false) => string.Empty,
                ("r", false, true) => "< {1}",
                ("r", true, false) => "> {0}",
                ("r", true, true) _ => "{0} - {1}",
                ("R", false, true) => "Less than {1}",
                ("R", true, false) => "Greater than {0}",
                ("R", true, true) => "Between {0} and {1}",
                _ => string.Empty,
            };
        }
    }

    public struct NameDateRange : IDateRange
    {
        public enum Names : ushort { Older, ThisYear, LastMonth, ThisMonth, LastWeek, ThisWeek, Yesterday, Today }

        public static NameDateRange All => new(Names.Older, Names.Today);

        public Date Today { get; }

        public Names MinName { get; }
        public Names MaxName { get; }

        public Date MinDate => MinName switch
        {
            Names.Today => Today,
            Names.Yesterday => Today.AddDays(-1),
            Names.ThisWeek => Today.AddDays(-6),
            Names.LastWeek => Today.AddDays(-13),
            Names.ThisMonth => Today.AddMonths(-1).AddDays(1),
            Names.LastMonth => Today.AddMonths(-2).AddDays(1),
            Names.ThisYear => Today.AddYears(-1).AddDays(1),
            Names.Older => Date.MinValue,
            _ => throw new ArgumentException(),
        };
        public Date MaxDate => MaxName switch
        {
            Names.Today => Today,
            Names.Yesterday => Today.AddDays(-1),
            Names.ThisWeek => Today.AddDays(-2),
            Names.LastWeek => Today.AddDays(-7),
            Names.ThisMonth => Today.AddDays(-14),
            Names.LastMonth => Today.AddMonths(-1),
            Names.ThisYear => Today.AddYears(-1),
            Names.Older => Date.MinValue,
            _ => throw new ArgumentException(),
        };

        public NameDateRange(Names name) => (Today, MinName, MaxName) = (Date.Today, name, name);
        public NameDateRange(Date today, Names name) => (Today, MinName, MaxName) = (today, name, name);
        public NameDateRange(Names minName, Names maxName) => (Today, MinName, MaxName) = (Date.Today, minName, maxName);
        public NameDateRange(Date today, Names minName, Names maxName) => (Today, MinName, MaxName) = (today, minName, maxName);

        public bool Equals(IDateRange other) => other is NameDateRange range && range.MinName == MinName && range.MaxName == MaxName;

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "r" || format == "R")
            {
                new DateRange(MinDate, MaxDate).ToString(format, formatProvider);
            }

            bool hasMin = MinName > Names.Older;
            bool hasMax = MaxName < Names.Today;

            if (format == "n" || format == "N")
            {
                if (MinName == MaxName)
                {
                    return GetLabel(MinName);
                }
                return string.Format(GetFormat(), GetLabel(MinName), GetLabel(MaxName));
            }

            return string.Empty;

            string GetLabel(Names name) => name switch
            {
                Names.Today => "ItemTimeText_Today".GetLocalized(),
                Names.Yesterday => "ItemTimeText_Yesterday".GetLocalized(),
                Names.ThisWeek => "ItemTimeText_ThisWeek".GetLocalized(),
                Names.LastWeek => "ItemTimeText_LastWeek".GetLocalized(),
                Names.ThisMonth => "ItemTimeText_ThisMonth".GetLocalized(),
                Names.LastMonth => "ItemTimeText_LastMonth".GetLocalized(),
                Names.ThisYear => "ItemTimeText_ThisYear".GetLocalized(),
                Names.Older => "ItemTimeText_Older".GetLocalized(),
                _ => throw new ArgumentException(),
            };
            string GetFormat() => (format, hasMin, hasMax) switch
            {
                (_, false, false) => string.Empty,
                ("n", false, true) => "< {1}",
                ("n", true, false) => "> {0}",
                ("n", true, true) _ => "{0} - {1}",
                ("N", false, true) => "Less than {1}",
                ("N", true, false) => "Greater than {0}",
                ("N", true, true) => "Between {0} and {1}",
                _ => string.Empty,
            };
        }
    }
}
