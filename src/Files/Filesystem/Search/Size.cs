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

        private string GetName() => size == ByteSize.MaxValue ? "ItemSizeText_NoLimit".GetLocalized() : null;
    }

    public struct SizeRange : IRange<Size>, IEquatable<IRange<Size>>
    {
        public static readonly SizeRange None
            = new(RangeDirections.None, Size.MaxValue, Size.MaxValue);
        public static readonly SizeRange All
            = new(RangeDirections.None, Size.MinValue, Size.MaxValue);
        public static readonly SizeRange Empty
            = new(RangeDirections.EqualTo, Size.MinValue, Size.MinValue, "ItemSizeText_Empty");
        public static readonly SizeRange Tiny
            = new(RangeDirections.Between, new Size(1), new Size(16, Size.Units.Kibi), "ItemSizeText_Tiny");
        public static readonly SizeRange Small
            = new(RangeDirections.Between, new Size(16, Size.Units.Kibi), new Size(1, Size.Units.Mebi), "ItemSizeText_Small");
        public static readonly SizeRange Medium
            = new(RangeDirections.Between, new Size(1, Size.Units.Mebi), new Size(128, Size.Units.Mebi), "ItemSizeText_Medium");
        public static readonly SizeRange Large
            = new(RangeDirections.Between, new Size(128, Size.Units.Mebi), new Size(1, Size.Units.Gibi), "ItemSizeText_Large");
        public static readonly SizeRange VeryLarge
            = new(RangeDirections.Between, new Size(1, Size.Units.Gibi), new Size(5, Size.Units.Gibi), "ItemSizeText_VeryLarge");
        public static readonly SizeRange Huge
            = new(RangeDirections.GreaterThan, new Size(5, Size.Units.Gibi), Size.MaxValue, "ItemSizeText_Huge");

        public bool IsNamed { get; }

        public RangeDirections Direction { get; }

        public Size MinValue { get; }
        public Size MaxValue { get; }

        public IRange<string> Label { get; }

        public SizeRange(Size minValue, Size maxValue)
        {
            (MinValue, MaxValue) = (minValue <= maxValue) ? (minValue, maxValue) : (maxValue, minValue);

            var names = new List<IRange<Size>> { Empty, Tiny, Small, Medium, Large, VeryLarge, Huge };
            IsNamed = names.Any(n => n.MinValue == minValue) && names.Any(n => n.MaxValue == maxValue);

            Direction = GetDirection(minValue, MaxValue);
            Label = GetLabel(IsNamed, Direction, MinValue, MaxValue);
        }
        private SizeRange(IRange<Size> minRange, IRange<Size> maxRange)
            : this(Min(minRange.MinValue, maxRange.MinValue), Max(minRange.MaxValue, maxRange.MaxValue)) {}
        private SizeRange(RangeDirections direction, Size minValue, Size maxValue)
            => (IsNamed, Direction, MinValue, MaxValue, Label) = (true, direction, minValue, maxValue, RangeLabel.None);
        private SizeRange(RangeDirections direction, Size minValue, Size maxValue, string labelKey)
            => (IsNamed, Direction, MinValue, MaxValue, Label) = (true, direction, minValue, maxValue, new RangeLabel(labelKey.GetLocalized()));

        public void Deconstruct(out Size minValue, out Size maxValue)
            => (minValue, maxValue) = (MinValue, MaxValue);
        public void Deconstruct(out RangeDirections direction, out Size minValue, out Size maxValue)
            => (direction, minValue, maxValue) = (Direction, MinValue, MaxValue);

        public override string ToString() => Label.ToString();

        public override int GetHashCode()
            => (MinValue, MaxValue).GetHashCode();
        public override bool Equals(object other)
            => other is SizeRange range && Equals(range);
        public bool Equals(IRange<Size> other)
            => other is SizeRange range && range.MinValue.Equals(MinValue) && range.MaxValue.Equals(MaxValue);

        public static SizeRange operator +(SizeRange a, IRange<Size> b) => new(a, b);
        public static SizeRange operator +(IRange<Size> a, SizeRange b) => new(a, b);
        public static SizeRange operator -(SizeRange a, IRange<Size> b) => Substract(a, b);
        public static SizeRange operator -(IRange<Size> a, SizeRange b) => Substract(a, b);
        public static bool operator ==(SizeRange a, IRange<Size> b) => a.Equals(b);
        public static bool operator ==(IRange<Size> a, SizeRange b) => a.Equals(b);
        public static bool operator !=(SizeRange a, IRange<Size> b) => !a.Equals(b);
        public static bool operator !=(IRange<Size> a, SizeRange b) => !a.Equals(b);
        public static bool operator <(SizeRange a, IRange<Size> b) => a.MaxValue < b.MinValue;
        public static bool operator <(IRange<Size> a, SizeRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(SizeRange a, IRange<Size> b) => a.MaxValue > b.MinValue;
        public static bool operator >(IRange<Size> a, SizeRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(SizeRange a, IRange<Size> b) => a.MaxValue <= b.MinValue;
        public static bool operator <=(IRange<Size> a, SizeRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(SizeRange a, IRange<Size> b) => a.MaxValue >= b.MinValue;
        public static bool operator >=(IRange<Size> a, SizeRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Size size) => size >= MinValue && size <= MaxValue;
        public bool Contains(IRange<Size> range) => range.MinValue >= MinValue && range.MaxValue <= MaxValue;

        private static Size Min(Size a, Size b) => a <= b ? a : b;
        private static Size Max(Size a, Size b) => a >= b ? a : b;

        private static SizeRange Substract(IRange<Size> a, IRange<Size> b)
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

        private static RangeDirections GetDirection(Size minValue, Size maxValue)
        {
            bool hasMin = minValue > Size.MinValue;
            bool hasMax = maxValue < Size.MaxValue;

            return (hasMin, hasMax) switch
            {
                (false, false) => RangeDirections.None,
                (true, false) => RangeDirections.GreaterThan,
                (false, true) => RangeDirections.LessThan,
                _ when minValue == maxValue => RangeDirections.EqualTo,
                _ => RangeDirections.Between,
            };
        }

        private static IRange<string> GetLabel(bool isNamed, RangeDirections direction, Size minValue, Size maxValue)
        {
            if (direction == RangeDirections.None)
            {
                return RangeLabel.None;
            }
            if (minValue == Empty.MinValue && maxValue == Empty.MaxValue)
            {
                return Empty.Label;
            }
            if (minValue == Huge.MinValue && maxValue == Huge.MaxValue)
            {
                return Huge.Label;
            }

            string minLabel = isNamed switch
            {
                _ when direction == RangeDirections.LessThan => string.Empty,
                true when Tiny.MinValue.Equals(minValue) => Tiny.Label.MinValue,
                true when Small.MinValue.Equals(minValue) => Small.Label.MinValue,
                true when Medium.MinValue.Equals(minValue) => Medium.Label.MinValue,
                true when Large.MinValue.Equals(minValue) => Large.Label.MinValue,
                true when VeryLarge.MinValue.Equals(minValue) => VeryLarge.Label.MinValue,
                true when Huge.MinValue.Equals(minValue) => Huge.Label.MinValue,
                false => $"{minValue}",
                _ => string.Empty,
            };
            string maxLabel = isNamed switch
            {
                _ when direction == RangeDirections.GreaterThan => string.Empty,
                true when Empty.MaxValue.Equals(maxValue) => Empty.Label.MaxValue,
                true when Tiny.MaxValue.Equals(maxValue) => Tiny.Label.MaxValue,
                true when Small.MaxValue.Equals(maxValue) => Small.Label.MaxValue,
                true when Medium.MaxValue.Equals(maxValue) => Medium.Label.MaxValue,
                true when Large.MaxValue.Equals(maxValue) => Large.Label.MaxValue,
                true when VeryLarge.MaxValue.Equals(maxValue) => VeryLarge.Label.MaxValue,
                false => $"{maxValue}",
                _ => string.Empty,
            };

            return new RangeLabel(minLabel, maxLabel);
        }
    }
}
