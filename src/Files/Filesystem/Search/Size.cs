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
        public static readonly SizeRange None = new(true, Size.MaxValue, Size.MaxValue);
        public static readonly SizeRange All = new(true, Size.MinValue, Size.MaxValue);
        public static readonly SizeRange Empty = new(true, Size.MinValue, Size.MinValue);
        public static readonly SizeRange Tiny = new(true, new Size(1), new Size(16, Size.Units.Kibi));
        public static readonly SizeRange Small = new(true, new Size(16, Size.Units.Kibi), new Size(1, Size.Units.Mebi));
        public static readonly SizeRange Medium = new(true, new Size(1, Size.Units.Mebi), new Size(128, Size.Units.Mebi));
        public static readonly SizeRange Large = new(true, new Size(128, Size.Units.Mebi), new Size(1, Size.Units.Gibi));
        public static readonly SizeRange VeryLarge = new(true, new Size(1, Size.Units.Gibi), new Size(5, Size.Units.Gibi));
        public static readonly SizeRange Huge = new(true, new Size(5, Size.Units.Gibi), Size.MaxValue);

        public bool IsNamed { get; }

        public Size MinValue { get; }
        public Size MaxValue { get; }

        public SizeRange(Size minValue, Size maxValue)
        {
            if (minValue > maxValue)
            {
                (minValue, maxValue) = (maxValue, minValue);
            }

            var named = new List<SizeRange> { Empty, Tiny, Small, Medium, Large, VeryLarge, Huge };
            bool isNamed = named.Any(n => n.MinValue == minValue) && named.Any(n => n.MaxValue == maxValue);

            (IsNamed, MinValue, MaxValue) = (isNamed, minValue, maxValue);
        }
        public SizeRange(Size minValue, SizeRange maxRange)
            : this(Min(minValue, maxRange.MinValue), Max(minValue, maxRange.MaxValue)) {}
        public SizeRange(SizeRange minRange, Size maxValue)
            : this(Min(minRange.MinValue, maxValue), Max(minRange.MaxValue, maxValue)) {}
        public SizeRange(SizeRange minRange, SizeRange maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        private SizeRange(bool isNamed, Size minValue, Size maxValue)
            => (IsNamed, MinValue, MaxValue) = (isNamed, minValue, maxValue);

        public void Deconstruct(out Size minValue, out Size maxValue)
            => (minValue, maxValue) = (MinValue, MaxValue);
        public void Deconstruct(out bool isNamed, out Size minValue, out Size maxValue)
            => (isNamed, minValue, maxValue) = (IsNamed, MinValue, MaxValue);

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is SizeRange range && Equals(range);
        public bool Equals(SizeRange other)
            => other is SizeRange range && (range.MinValue, range.MaxValue) == (MinValue, MaxValue);

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (Equals(None) || Equals(All))
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

            bool hasMin = minValue > Size.MinValue;
            bool hasMax = maxValue < Size.MaxValue;

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
                true when Empty.MinValue.Equals(minValue) => "SearchSizeRange_Empty".GetLocalized(),
                true when Tiny.MinValue.Equals(minValue) => "ItemSizeText_Tiny".GetLocalized(),
                true when Small.MinValue.Equals(minValue) => "ItemSizeText_Small".GetLocalized(),
                true when Medium.MinValue.Equals(minValue) => "ItemSizeText_Medium".GetLocalized(),
                true when Large.MinValue.Equals(minValue) => "ItemSizeText_Large".GetLocalized(),
                true when VeryLarge.MinValue.Equals(minValue) => "ItemSizeText_VeryLarge".GetLocalized(),
                true when Huge.MinValue.Equals(minValue) => "ItemSizeText_Huge".GetLocalized(),
                true => string.Empty,
                false => $"{minValue}",
            };
            string GetMaxLabel() => useName switch
            {
                true when Empty.MaxValue.Equals(maxValue) => "SearchSizeRange_Empty".GetLocalized(),
                true when Tiny.MaxValue.Equals(maxValue) => "ItemSizeText_Tiny".GetLocalized(),
                true when Small.MaxValue.Equals(maxValue) => "ItemSizeText_Small".GetLocalized(),
                true when Medium.MaxValue.Equals(maxValue) => "ItemSizeText_Medium".GetLocalized(),
                true when Large.MaxValue.Equals(maxValue) => "ItemSizeText_Large".GetLocalized(),
                true when VeryLarge.MaxValue.Equals(maxValue) => "ItemSizeText_VeryLarge".GetLocalized(),
                true when Huge.MaxValue.Equals(maxValue) => "ItemSizeText_Huge".GetLocalized(),
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
                (false, _) => "SearchSizeRange_LessThan".GetLocalized(),
                (_, false) => "SearchSizeRange_GreaterThan".GetLocalized(),
                _ => "SearchSizeRange_Between".GetLocalized(),
            };
            string GetQueryFormat() => (hasMin, hasMax) switch
            {
                _ when minValue == maxValue => "{0:B}",
                (false, _) => "<{1:B}",
                (_, false) => ">{0:B}",
                _ => "{0:B}..{1:B}",
            };
        }

        public static SizeRange operator +(SizeRange a, SizeRange b) => new(a, b);
        public static SizeRange operator -(SizeRange a, SizeRange b) => Substract(a, b);
        public static bool operator ==(SizeRange a, SizeRange b) => a.Equals(b);
        public static bool operator !=(SizeRange a, SizeRange b) => !a.Equals(b);
        public static bool operator <(SizeRange a, SizeRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(SizeRange a, SizeRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(SizeRange a, SizeRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(SizeRange a, SizeRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Size size) => MinValue <= size && size <= MaxValue;
        public bool Contains(SizeRange range) => MinValue <= range.MinValue && range.MaxValue <= MaxValue;

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
