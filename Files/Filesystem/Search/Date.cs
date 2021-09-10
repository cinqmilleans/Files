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

        public Date(ushort year, ushort month, ushort day) : this(new DateTime(year, month, day)) {}
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
        public Date MinDate { get; }
        public Date MaxDate { get; }

        public DateRange() : this(Date.MinValue, Date.MaxValue) {}
        public DateRange(Date minDate, Date maxDate) => (MinDate, MaxDate) = minDate <= maxDate ? (minDate, maxDate) : (maxDate, minDate);
        protected DateRange((Date minDate, Date maxDate) range) : this(range.minDate, range.maxDate) {}

        public void Deconstruct(out Date minDate, out Date maxDate) => (minDate, maxDate) = (MinDate, MaxDate);

        public override int GetHashCode() => (MinDate, MaxDate).GetHashCode();
        public override bool Equals(object other) => other is DateRange range && Equals(range);
        public virtual bool Equals(DateRange other) => (other.MinDate, other.MaxDate).Equals((MinDate, MaxDate));

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            bool hasMinDate = MinDate != Date.MinValue && MaxDate != Date.MaxValue;
            bool hasMaxDate = MaxDate != Date.MaxValue && MaxDate != Date.MaxValue;

            return format switch
            {
                "r" => string.Format(GetShortFormat(), $"{MinDate:d}", $"{MaxDate:d}"),
                "R" => string.Format(GetFullFormat(), $"{MinDate:d}", $"{MaxDate:d}"),
                _ => string.Empty,
            };

            string GetShortFormat() => (hasMinDate, hasMaxDate) switch
            {
                (false, false) => string.Empty,
                (true, false) => "< {1}",
                (false, true) => "> {1}",
                _ when MinDate == MaxDate => "{1}",
                _ => "{0} - {1}",
            };
            string GetFullFormat() => (hasMinDate, hasMaxDate) switch
            {
                (false, false) => string.Empty,
                (true, false) => "Before {1}",
                (false, true) => "After {1}",
                _ when MinDate == MaxDate => "{1}",
                _ => "Between {0} and {1}",
            };
        }
    }

    public class NamedDateRange : DateRange, IEquatable<NamedDateRange>
    {
        public enum Names { Unnamed, Always, Today, Yesterday, ThisWeek, LastWeek, ThisMonth, LastMonth, ThisYear, LastYear }

        public Names Name { get; }

        public NamedDateRange() : this(Names.Always) {}
        public NamedDateRange(Names name) : this(name, Date.Today) {}
        public NamedDateRange(Names name, Date today) : base(GetRange(name, today)) => Name = name;
        public NamedDateRange(Date minDate, Date maxDate) : this(minDate, maxDate, Date.Today) {}
        public NamedDateRange(Date minDate, Date maxDate, Date today) : base(minDate, maxDate) => Name = GetName(minDate, maxDate, today);

        public void Deconstruct(out Names name, out Date minDate, out Date maxDate) => (name, minDate, maxDate) = (Name, MinDate, MaxDate);

        public override int GetHashCode() => (Name, MinDate, MaxDate).GetHashCode();
        public override bool Equals(object other) => other switch
        {
            NamedDateRange range => Equals(range),
            DateRange range => Equals(range),
            _ => false,
        };
        public virtual bool Equals(NamedDateRange other) => (other.Name, other.MinDate, other.MaxDate).Equals((Name, MinDate, MaxDate));

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            return format switch
            {
                "n" => GetLabel(Name) ?? base.ToString("r", formatProvider),
                "N" => GetLabel(Name) ?? base.ToString("R", formatProvider),
                _ => base.ToString(format, formatProvider),
            };

            static string GetLabel(Names name) => name switch
            {
                Names.Always => string.Empty,
                Names.Today => "ItemDateText_Today".GetLocalized(),
                Names.Yesterday => "ItemDateText_Yesterday".GetLocalized(),
                Names.ThisWeek => "ItemDateText_ThisWeek".GetLocalized(),
                Names.LastWeek => "ItemDateText_LastWeek".GetLocalized(),
                Names.ThisMonth => "ItemDateText_ThisMonth".GetLocalized(),
                Names.LastMonth => "ItemDateText_LastMonth".GetLocalized(),
                Names.ThisYear => "ItemDateText_ThisYear".GetLocalized(),
                Names.LastYear => "ItemDateText_Older".GetLocalized(),
                _ => null,
            };
        }

        private static Names GetName(Date minDate, Date maxDate, Date today)
        {
            var names = Enum.GetValues(typeof(Names)).Cast<Names>();
            foreach (var name in names)
            {
                var range = new NamedDateRange(name, today);
                if ((range.MinDate, range.MaxDate).Equals((minDate, maxDate)))
                {
                    return name;
                }
            }
            return Names.Unnamed;
        }

        private static (Date, Date) GetRange(Names name, Date today) => name switch
        {
            Names.Always => (Date.MinValue, Date.MaxValue),
            Names.Today => (today, today),
            Names.Yesterday => (today.AddDays(-1), today.AddDays(-1)),
            Names.ThisWeek => (today.AddDays(-6), today),
            Names.LastWeek => (today.AddDays(-13), today.AddDays(-7)),
            Names.ThisMonth => (today.AddMonths(-1).AddDays(1), today),
            Names.LastMonth => (today.AddMonths(-2).AddDays(1), today.AddMonths(-1)),
            Names.ThisYear => (today.AddYears(-1).AddDays(1), today),
            Names.LastYear => (today.AddYears(-2).AddDays(1), today.AddYears(-1)),
            _ => throw new ArgumentException(),
        };
    }
}
