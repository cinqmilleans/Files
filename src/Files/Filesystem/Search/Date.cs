using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Date : IEquatable<Date>, IComparable<Date>, IFormattable
    {
        public static event EventHandler TodayUpdated;

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

        public Date AddDays(int days) => new(date.AddDays(days));
        public Date AddMonths(int months) => new(date.AddMonths(months));
        public Date AddYears(int years) => new(date.AddYears(years));

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

        public static void UpdateToday()
        {
            var oldToday = Today;
            var newToday = new Date(DateTime.Today);

            if (oldToday < newToday)
            {
                today = new(DateTime.Today);
                TodayUpdated.Invoke(null, EventArgs.Empty);
            }
        }
    }

    public struct DateRange : IRange<Date>, IEquatable<IRange<Date>>
    {
        private readonly IRange range;

        public static readonly DateRange None = new(new NoneRange());
        public static readonly DateRange Always = new(Date.MinValue, Date.MaxValue);

        public static readonly DateRange Today = new(new RelativeRange(RelativeRange.Moments.Today));
        public static readonly DateRange Yesterday = new(new RelativeRange(RelativeRange.Moments.Yesterday));
        public static readonly DateRange ThisWeek = new(new RelativeRange(RelativeRange.Moments.ThisWeek));
        public static readonly DateRange LastWeek = new(new RelativeRange(RelativeRange.Moments.LastWeek));
        public static readonly DateRange ThisMonth = new(new RelativeRange(RelativeRange.Moments.ThisMonth));
        public static readonly DateRange LastMonth = new(new RelativeRange(RelativeRange.Moments.LastMonth));
        public static readonly DateRange ThisYear = new(new RelativeRange(RelativeRange.Moments.ThisYear));
        public static readonly DateRange Older = new(new RelativeRange(RelativeRange.Moments.Older));

        public bool IsRelative => range is RelativeRange;

        public RangeDirections Direction => range.Direction;

        public Date MinValue => range.MinValue;
        public Date MaxValue => range.MaxValue;

        public IRange<string> Label => range.Label;

        public DateRange(Date minDate, Date maxDate) => range = GetRange(minDate, maxDate);
        private DateRange(IRange range) => this.range = range;

        public void Deconstruct(out Date minValue, out Date maxValue)
            => (_, minValue, maxValue) = range;
        public void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue)
            => (direction, minValue, maxValue) = range;

        public override string ToString() => Label.ToString();

        public override int GetHashCode() => range.GetHashCode();
        public override bool Equals(object other) => range.Equals(other);
        public bool Equals(IRange<Date> other) => range.Equals(other);

        public static DateRange operator +(DateRange a, DateRange b)
        {
            var (minDateA, maxDateA) = a;
            var (minDateB, maxDateB) = b;

            Date minDate = minDateA <= minDateB ? minDateA : minDateB;
            Date maxDate = maxDateB >= maxDateA ? maxDateB : maxDateA;

            return new(minDate, maxDate);
        }
        public static DateRange operator -(DateRange a, DateRange b)
        {
            var (minDateA, maxDateA) = a;
            var (minDateB, maxDateB) = b;

            if (minDateB == minDateA && maxDateB < maxDateA)
            {
                return new(maxDateB.AddDays(1), maxDateA);
            }
            if (maxDateB == maxDateA && minDateB > minDateA)
            {
                return new(minDateA, minDateB.AddDays(-1));
            }
            return None;
        }
        public static bool operator ==(DateRange a, DateRange b) => a.Equals(b);
        public static bool operator !=(DateRange a, DateRange b) => !a.Equals(b);
        public static bool operator <(DateRange a, DateRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(DateRange a, DateRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(DateRange a, DateRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(DateRange a, DateRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Date size) => size >= MinValue && size <= MaxValue;
        public bool Contains(DateRange range) => range.MinValue >= MinValue && range.MaxValue <= MaxValue;

        private static IRange GetRange(Date minDate, Date maxDate)
        {
            Date today = Date.Today;

            if (minDate > maxDate)
            {
                (minDate, maxDate) = (maxDate, minDate);
            }
            if (minDate > today)
            {
                minDate = today;
            }
            if (maxDate > today)
            {
                maxDate = today;
            }

            var moments = RelativeRange.MomentRanges.ToList();

            bool hasMinMoment = moments.Any(n => n.MinDate == minDate);
            bool hasMaxMoment = moments.Any(n => n.MaxDate == maxDate);

            if (!hasMinMoment || !hasMaxMoment)
                return new DayRange(minDate, maxDate);

            RelativeRange minMoment = moments.First(n => n.MinDate == minDate).Range;
            RelativeRange maxMoment = moments.First(n => n.MaxDate == maxDate).Range;

            return new RelativeRange(minMoment, maxMoment);
        }

        private interface IRange : IRange<Date>, IEquatable<IRange<Date>>
        {
            void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue);
        }

        private struct NoneRange : IRange
        {
            public RangeDirections Direction => RangeDirections.None;

            public Date MinValue => Date.MaxValue;
            public Date MaxValue => Date.MinValue;

            public IRange<string> Label => RangeLabel.None;

            public void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue)
                => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);

            public override int GetHashCode() => Direction.GetHashCode();
            public override bool Equals(object other) => other is NoneRange;
            public bool Equals(IRange<Date> other) => other is NoneRange;
        }

        private struct RelativeRange : IRange
        {
            public enum Moments : ushort { Today, Yesterday, ThisWeek, LastWeek, ThisMonth, LastMonth, ThisYear, Older }

            public static IEnumerable<(RelativeRange Range, Date MinDate, Date MaxDate)> MomentRanges
            {
                get
                {
                    Date today = Date.Today;

                    return Enum.GetValues(typeof(Moments)).Cast<Moments>()
                        .Select(moment => new RelativeRange(moment))
                        .Select(range => (range, range.GetMinDate(today), range.GetMinDate(today)));
                }
            }

            public readonly Moments MinMoment { get; }
            public readonly Moments MaxMoment { get; }

            public RangeDirections Direction => (MinMoment, MaxMoment) switch
            {
                (Moments.Older, Moments.Today) => RangeDirections.None,
                _ when MinMoment == MaxMoment => RangeDirections.EqualTo,
                (Moments.Older, _) => RangeDirections.LessThan,
                (_, Moments.Today) => RangeDirections.GreaterThan,
                _ => RangeDirections.Between,
            };

            public Date MinValue => GetMinDate(Date.Today);
            public Date MaxValue => GetMaxDate(Date.Today);

            public IRange<string> Label => new RangeLabel(GetText(MinMoment), GetText(MaxMoment));

            public RelativeRange(Moments moment) => MinMoment = MaxMoment = moment;
            public RelativeRange(RelativeRange minRange, RelativeRange maxRange)
            {
                MinMoment = minRange.MinMoment <= maxRange.MinMoment ? minRange.MinMoment : maxRange.MinMoment;
                MaxMoment = maxRange.MaxMoment >= minRange.MaxMoment ? maxRange.MaxMoment : minRange.MaxMoment;
            }

            public void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue)
            {
                Date today = Date.Today;
                direction = Direction;
                minValue = GetMinDate(today);
                maxValue = GetMaxDate(today);
            }

            public override int GetHashCode()
                => (MinMoment, MaxMoment).GetHashCode();
            public override bool Equals(object other)
                => other is RelativeRange range && Equals(range);
            public bool Equals(IRange<Date> other)
                => other is RelativeRange range && range.MinMoment == MinMoment && range.MaxMoment == MaxMoment;

            private static string GetText(Moments moment) => moment switch
            {
                Moments.Today => "ItemTimeText_Today".GetLocalized(),
                Moments.Yesterday => "ItemTimeText_Yesterday".GetLocalized(),
                Moments.ThisWeek => "ItemTimeText_ThisWeek".GetLocalized(),
                Moments.LastWeek => "ItemTimeText_LastWeek".GetLocalized(),
                Moments.ThisMonth => "ItemTimeText_ThisMonth".GetLocalized(),
                Moments.LastMonth => "ItemTimeText_LastMonth".GetLocalized(),
                Moments.ThisYear => "ItemTimeText_ThisYear".GetLocalized(),
                Moments.Older => "ItemTimeText_Older".GetLocalized(),
                _ => throw new ArgumentException(),
            };
            private Date GetMinDate(Date today) => MinMoment switch
            {
                Moments.Today => today,
                Moments.Yesterday => today.AddDays(-1),
                Moments.ThisWeek => today.AddDays(-6),
                Moments.LastWeek => today.AddDays(-13),
                Moments.ThisMonth => today.AddMonths(-1).AddDays(1),
                Moments.LastMonth => today.AddMonths(-2).AddDays(1),
                Moments.ThisYear => today.AddYears(-1).AddDays(1),
                Moments.Older => Date.MinValue,
                _ => throw new ArgumentException(),
            };
            private Date GetMaxDate(Date today) => MaxMoment switch
            {
                Moments.Today => today,
                Moments.Yesterday => today.AddDays(-1),
                Moments.ThisWeek => today.AddDays(-2),
                Moments.LastWeek => today.AddDays(-7),
                Moments.ThisMonth => today.AddDays(-14),
                Moments.LastMonth => today.AddMonths(-1),
                Moments.ThisYear => today.AddMonths(-2),
                Moments.Older => today.AddYears(-1),
                _ => throw new ArgumentException(),
            };
        }

        public struct DayRange : IRange
        {
            private static readonly ILabelBuilder labelBuilder = new LabelBuilderCollection
            {
                new YearBuilder(),
                new DayBuilder(),
            };

            public RangeDirections Direction
            {
                get
                {
                    bool hasMin = MinValue > Date.MinValue;
                    bool hasMax = MaxValue < Date.Today;

                    return (hasMin, hasMax) switch
                    {
                        (false, false) => RangeDirections.None,
                        _ when MinValue == MaxValue => RangeDirections.EqualTo,
                        (true, false) => RangeDirections.GreaterThan,
                        (false, true) => RangeDirections.LessThan,
                        _ => RangeDirections.Between,
                    };
                }
            }

            public Date MinValue { get; }
            public Date MaxValue { get; }

            public IRange<string> Label
            {
                get
                {
                    Date? minDate = MinValue > Date.MinValue ? MinValue : null;
                    Date? maxDate = MaxValue < Date.Today ? MaxValue : null;

                    if (labelBuilder.CanBuild(minDate, maxDate))
                    {
                        return labelBuilder.Build(minDate, maxDate);
                    }
                    return RangeLabel.None;
                }
            }

            public DayRange(Date minDate, Date maxDate)
            {
                MinValue = minDate <= maxDate ? minDate : maxDate;
                MaxValue = maxDate >= minDate ? maxDate : minDate;
            }

            public void Deconstruct(out RangeDirections direction, out Date minValue, out Date maxValue)
                => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);

            public override int GetHashCode()
                => (MinValue, MaxValue).GetHashCode();
            public override bool Equals(object other)
                => other is DayRange range && Equals(range);
            public bool Equals(IRange<Date> other)
                => other is DayRange range && range.MinValue == MinValue && range.MaxValue == MaxValue;

            private interface ILabelBuilder
            {
                bool CanBuild(Date? minDate, Date? maxDate);
                IRange<string> Build(Date? minDate, Date? maxDate);
            }

            private class LabelBuilderCollection : Collection<ILabelBuilder>, ILabelBuilder
            {
                public LabelBuilderCollection() : base() {}
                public LabelBuilderCollection(IList<ILabelBuilder> builders) : base(builders) {}

                public bool CanBuild(Date? minDate, Date? maxDate)
                    => this.Any(builder => builder.CanBuild(minDate, maxDate));
                public IRange<string> Build(Date? minDate, Date? maxDate)
                    => this.First(builder => builder.CanBuild(minDate, maxDate)).Build(minDate, maxDate);
            }

            private class YearBuilder : ILabelBuilder
            {
                public bool CanBuild(Date? minDate, Date? maxDate)
                {
                    bool hasMin = !minDate.HasValue || (minDate.Value.Month == 1 && minDate.Value.Day == 1);
                    bool hasMax = !maxDate.HasValue || (maxDate.Value.Month == 12 && minDate.Value.Day == 31);

                    return hasMin && hasMax;
                }
                public IRange<string> Build(Date? minDate, Date? maxDate)
                {
                    return new RangeLabel(GetText(minDate), GetText(maxDate));

                    static string GetText(Date? date) => date.HasValue ? date.Value.Year.ToString() : string.Empty;
                }
            }

            private class DayBuilder : ILabelBuilder
            {
                public bool CanBuild(Date? minDate, Date? maxDate) => true;
                public IRange<string> Build(Date? minDate, Date? maxDate)
                {
                    return new RangeLabel(GetText(minDate), GetText(maxDate));

                    static string GetText(Date? date) => date.HasValue ? date.Value.ToString() : string.Empty;
                }
            }
        }
    }
}
