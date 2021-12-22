using System;
using System.Globalization;

namespace Files.Filesystem.Search
{
    public enum RangeDirections : ushort
    {
        None,
        EqualTo,
        GreaterThan,
        LessThan,
        Between,
    }

    public interface IRange<out T>
    {
        RangeDirections Direction { get; }

        T MinValue { get; }
        T MaxValue { get; }

        IRange<string> Label { get; }
    }

    public struct RangeLabel : IRange<string>, IFormattable
    {
        public static RangeLabel None { get; } = new(string.Empty, string.Empty);

        public RangeDirections Direction { get; }

        public string MinValue { get; }
        public string MaxValue { get; }

        public IRange<string> Label => this;

        public RangeLabel(string value) : this(value, value) {}
        public RangeLabel(string minValue, string maxValue)
        {
            MinValue = (minValue ?? string.Empty).Trim();
            MaxValue = (maxValue ?? string.Empty).Trim();

            Direction = (MinValue, MaxValue) switch
            {
                ("", "") => RangeDirections.None,
                (_, "") => RangeDirections.GreaterThan,
                ("", _) => RangeDirections.LessThan,
                _ when MinValue == MaxValue => RangeDirections.EqualTo,
                _ => RangeDirections.Between,
            };
        }

        public void Deconstruct(out RangeDirections direction, out string minValue, out string maxValue)
            => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);

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

            string text = format switch
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
    }
}
