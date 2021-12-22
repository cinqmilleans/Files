using ByteSizeLib;
using Files.Extensions;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Size : IEquatable<Size>, IComparable<Size>, IFormattable
    {
        public enum Units : ushort { Byte, Kibi, Mebi, Gibi, Tebi, Pebi }

        private readonly ByteSize size;

        public static readonly Size MinValue = new(0);
        public static readonly Size MaxValue = new(ByteSize.MaxValue);

        public long Bytes => size.Bits * ByteSize.BitsInByte;

        public double Value => size.LargestWholeNumberBinaryValue;
        public Units Unit => size.LargestWholeNumberBinarySymbol switch
        {
            ByteSize.KibiByteSymbol => Units.Kibi,
            ByteSize.MebiByteSymbol => Units.Mebi,
            ByteSize.GibiByteSymbol => Units.Gibi,
            ByteSize.TebiByteSymbol => Units.Tebi,
            ByteSize.PebiByteSymbol => Units.Pebi,
            _ => Units.Byte,
        };

        public Size(long bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentException("Size is always positive.");
            }
            size = ByteSize.FromBytes(bytes);
        }
        public Size(double value, Units unit)
        {
            if (value < 0)
            {
                throw new ArgumentException("Size is always positive.");
            }
            size = unit switch
            {
                Units.Byte => ByteSize.FromBytes(value),
                Units.Kibi => ByteSize.FromKibiBytes(value),
                Units.Mebi => ByteSize.FromMebiBytes(value),
                Units.Gibi => ByteSize.FromGibiBytes(value),
                Units.Tebi => ByteSize.FromTebiBytes(value),
                Units.Pebi => ByteSize.FromPebiBytes(value),
                _ => throw new ArgumentException(),
            };
        }
        private Size(ByteSize size)
        {
            if (size.Bytes < 0)
            {
                throw new ArgumentException("Size is always positive.");
            }
            this.size = size;
        }

        public static implicit operator Size(ByteSize size) => new(size);
        public static explicit operator ByteSize(Size size) => size.size;

        public static Size operator +(Size a, Size b) => new(a.size + b.size);
        public static Size operator -(Size a, Size b) => new(a.size - b.size);
        public static bool operator ==(Size a, Size b) => a.size == b.size;
        public static bool operator !=(Size a, Size b) => a.size != b.size;
        public static bool operator <(Size a, Size b) => a.size < b.size;
        public static bool operator >(Size a, Size b) => a.size > b.size;
        public static bool operator <=(Size a, Size b) => a.size <= b.size;
        public static bool operator >=(Size a, Size b) => a.size >= b.size;

        public override int GetHashCode() => size.GetHashCode();
        public override bool Equals(object other) => other is Size size && Equals(size);
        public bool Equals(Size other) => other.size.Equals(size);
        public int CompareTo(Size other) => other.size.CompareTo(size);

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider) => (format ?? "G").ToLower() switch
        {
            "g" => size.ToBinaryString().ConvertSizeAbbreviation(),
            "G" => size.ToBinaryString().ConvertSizeAbbreviation(),
            "n" => GetName() ?? ToString("G", formatProvider),
            "N" => GetName() ?? ToString("G", formatProvider),
            "b" => Bytes.ToString(),
            "B" => Bytes.ToString(),
            "u" => size.LargestWholeNumberBinarySymbol.ConvertSizeAbbreviation(),
            "U" => size.LargestWholeNumberBinarySymbol.ConvertSizeAbbreviation(),
            _ => string.Empty,
        };

        private string GetName() => size == ByteSize.MaxValue ? "SearchSize_NoLimit".GetLocalized() : null;
    }

    public struct SizeRange : IRange<Size>, IEquatable<SizeRange>, IFormattable
    {
        public static readonly SizeRange None = new(true, RangeDirections.None, Size.MaxValue, Size.MaxValue);
        public static readonly SizeRange All = new(true, RangeDirections.None, Size.MinValue, Size.MaxValue);
        public static readonly SizeRange Empty = new(true, RangeDirections.EqualTo, Size.MinValue, Size.MinValue);
        public static readonly SizeRange Tiny = new(true, RangeDirections.Between, new Size(1), new Size(16, Size.Units.Kibi));
        public static readonly SizeRange Small = new(true, RangeDirections.Between, new Size(16, Size.Units.Kibi), new Size(1, Size.Units.Mebi));
        public static readonly SizeRange Medium = new(true, RangeDirections.Between, new Size(1, Size.Units.Mebi), new Size(128, Size.Units.Mebi));
        public static readonly SizeRange Large = new(true, RangeDirections.Between, new Size(128, Size.Units.Mebi), new Size(1, Size.Units.Gibi));
        public static readonly SizeRange VeryLarge = new(true, RangeDirections.Between, new Size(1, Size.Units.Gibi), new Size(5, Size.Units.Gibi));
        public static readonly SizeRange Huge = new(true, RangeDirections.GreaterThan, new Size(5, Size.Units.Gibi), Size.MaxValue);

        public bool IsNamed { get; }

        public RangeDirections Direction { get; }

        public Size MinValue { get; }
        public Size MaxValue { get; }

        public IRange<string> Label => ToLabel();

        public SizeRange(Size minValue, Size maxValue)
        {
            if (minValue > maxValue)
            {
                (minValue, maxValue) = (maxValue, minValue);
            }

            bool hasMin = minValue > Size.MinValue;
            bool hasMax = maxValue < Size.MaxValue;

            var direction = (hasMin, hasMax) switch
            {
                (false, false) => RangeDirections.None,
                (true, false) => RangeDirections.GreaterThan,
                (false, true) => RangeDirections.LessThan,
                _ when minValue == maxValue => RangeDirections.EqualTo,
                _ => RangeDirections.Between,
            };

            var named = new List<SizeRange> { Empty, Tiny, Small, Medium, Large, VeryLarge, Huge };
            bool isNamed = named.Any(n => n.MinValue == minValue) && named.Any(n => n.MaxValue == maxValue);

            (IsNamed, Direction, MinValue, MaxValue) = (isNamed, direction, minValue, maxValue);
        }
        public SizeRange(Size minValue, SizeRange maxRange)
            : this(Min(minValue, maxRange.MinValue), Max(minValue, maxRange.MaxValue)) {}
        public SizeRange(SizeRange minRange, Size maxValue)
            : this(Min(minRange.MinValue, maxValue), Max(minRange.MaxValue, maxValue)) {}
        public SizeRange(SizeRange minRange, SizeRange maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        private SizeRange(bool isNamed, RangeDirections direction, Size minValue, Size maxValue)
            => (IsNamed, Direction, MinValue, MaxValue) = (isNamed, direction, minValue, maxValue);

        public void Deconstruct(out bool isNamed, out RangeDirections direction, out Size minValue, out Size maxValue)
            => (isNamed, direction, minValue, maxValue) = (IsNamed, Direction, MinValue, MaxValue);

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is SizeRange range && Equals(range);
        public bool Equals(SizeRange other)
            => other is SizeRange range && range.MinValue == MinValue && range.MaxValue == MaxValue;

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
                _ => string.Empty
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
                RangeDirections.LessThan => "SearchSizeRange_LessThan".GetLocalized(),
                RangeDirections.GreaterThan => "SearchSizeRange_GreaterThan".GetLocalized(),
                _ => "SearchSizeRange_Between".GetLocalized(),
            };
            string GetQueryFormat() => direction switch
            {
                RangeDirections.EqualTo => "{0:B}",
                RangeDirections.LessThan => "<{1:B}",
                RangeDirections.GreaterThan => ">{0:B}",
                _ => "{0:B}..{1:B}",
            };
        }

        public RangeLabel ToLabel(bool useName = true)
        {
            useName &= IsNamed;

            if (Direction == RangeDirections.None)
            {
                return RangeLabel.None;
            }
            if (useName && Equals(Empty))
            {
                return new RangeLabel("ItemSizeText_Empty".GetLocalized());
            }
            if (useName && Equals(Huge))
            {
                return new RangeLabel("ItemSizeText_Huge".GetLocalized());
            }

            string minLabel = useName switch
            {
                _ when Direction == RangeDirections.LessThan => string.Empty,
                true when Tiny.MinValue.Equals(MinValue) => "ItemSizeText_Tiny".GetLocalized(),
                true when Small.MinValue.Equals(MinValue) => "ItemSizeText_Small".GetLocalized(),
                true when Medium.MinValue.Equals(MinValue) => "ItemSizeText_Medium".GetLocalized(),
                true when Large.MinValue.Equals(MinValue) => "ItemSizeText_Large".GetLocalized(),
                true when VeryLarge.MinValue.Equals(MinValue) => "ItemSizeText_VeryLarge".GetLocalized(),
                true when Huge.MinValue.Equals(MinValue) => "ItemSizeText_Huge".GetLocalized(),
                false => $"{MinValue}",
                _ => string.Empty,
            };
            string maxLabel = useName switch
            {
                _ when Direction == RangeDirections.GreaterThan => string.Empty,
                true when Empty.MaxValue.Equals(MaxValue) => "ItemSizeText_Empty".GetLocalized(),
                true when Tiny.MaxValue.Equals(MaxValue) => "ItemSizeText_Tiny".GetLocalized(),
                true when Small.MaxValue.Equals(MaxValue) => "ItemSizeText_Small".GetLocalized(),
                true when Medium.MaxValue.Equals(MaxValue) => "ItemSizeText_Medium".GetLocalized(),
                true when Large.MaxValue.Equals(MaxValue) => "ItemSizeText_Large".GetLocalized(),
                true when VeryLarge.MaxValue.Equals(MaxValue) => "ItemSizeText_VeryLarge".GetLocalized(),
                false => $"{MaxValue}",
                _ => string.Empty,
            };

            return new RangeLabel(minLabel, maxLabel);
        }

        public static SizeRange operator +(SizeRange a, SizeRange b) => new(a, b);
        public static SizeRange operator -(SizeRange a, SizeRange b) => Substract(a, b);
        public static bool operator ==(SizeRange a, SizeRange b) => a.Equals(b);
        public static bool operator !=(SizeRange a, SizeRange b) => !a.Equals(b);
        public static bool operator <(SizeRange a, SizeRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(SizeRange a, SizeRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(SizeRange a, SizeRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(SizeRange a, SizeRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Size size) => size >= MinValue && size <= MaxValue;
        public bool Contains(SizeRange range) => range.MinValue >= MinValue && range.MaxValue <= MaxValue;

        private static Size Min(Size a, Size b) => a <= b ? a : b;
        private static Size Max(Size a, Size b) => a >= b ? a : b;

        private static SizeRange Substract(SizeRange a, SizeRange b)
        {
            if (b.MinValue == a.MinValue && b.MaxValue < a.MaxValue)
            {
                return new(b.MaxValue, a.MaxValue);
            }
            if (b.MaxValue == a.MaxValue && b.MinValue > a.MinValue)
            {
                return new(a.MinValue, b.MinValue);
            }
            return None;
        }
    }
}
