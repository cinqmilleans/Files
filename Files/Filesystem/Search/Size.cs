using ByteSizeLib;
using Files.Extensions;
using Microsoft.Toolkit.Uwp;
using System;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Size : IEquatable<Size>, IComparable<Size>, IFormattable
    {
        public enum Units { Byte, Kibi, Mebi, Gibi, Tebi, Pebi }

        private readonly ByteSize size;

        public static Size MinValue { get; } = new Size(0);
        public static Size MaxValue { get; } = new Size(ByteSize.MaxValue);

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

        public Size(long bytes) => size = ByteSize.FromBytes(bytes);
        public Size(double value, Units unit) => size = unit switch
        {
            Units.Byte => ByteSize.FromBytes(value),
            Units.Kibi => ByteSize.FromKibiBytes(value),
            Units.Mebi => ByteSize.FromMebiBytes(value),
            Units.Gibi => ByteSize.FromGibiBytes(value),
            Units.Tebi => ByteSize.FromTebiBytes(value),
            Units.Pebi => ByteSize.FromPebiBytes(value),
            _ => throw new ArgumentException(),
        };
        private Size(ByteSize size) => this.size = size;

        public static Size operator +(Size s1, Size s2) => new Size(s1.size + s2.size);
        public static Size operator -(Size s1, Size s2) => new Size(s1.size - s2.size);
        public static bool operator ==(Size s1, Size s2) => s1.size == s2.size;
        public static bool operator !=(Size s1, Size s2) => s1.size != s2.size;
        public static bool operator <(Size s1, Size s2) => s1.size < s2.size;
        public static bool operator >(Size s1, Size s2) => s1.size > s2.size;
        public static bool operator <=(Size s1, Size s2) => s1.size <= s2.size;
        public static bool operator >=(Size s1, Size s2) => s1.size >= s2.size;

        public override int GetHashCode() => size.GetHashCode();
        public override bool Equals(object other) => other is Size size && Equals(size);
        public bool Equals(Size other) => other.size.Equals(size);
        public int CompareTo(Size other) => other.size.CompareTo(size);

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider) => (format ?? "G") switch
        {
            "G" => size.ToBinaryString().ConvertSizeAbbreviation(),
            "N" => GetName() ?? ToString("G", formatProvider),
            "U" => size.LargestWholeNumberBinarySymbol.ConvertSizeAbbreviation(),
            _ => string.Empty,
        };

        private string GetName() => size == ByteSize.MaxValue ? "No limit" : null;
    }

    public interface ISizeRange : IEquatable<ISizeRange>, IFormattable
    {
        Size MinSize { get; }
        Size MaxSize { get; }

        string ToString(string format);
    }

    public interface ISizeRangeFactory
    {
        ISizeRange Build();
        ISizeRange Build(NameSizeRange.Names name);
        ISizeRange Build(NameSizeRange.Names minName, NameSizeRange.Names maxName);
        ISizeRange Build(Size size);
        ISizeRange Build(Size minSize, Size maxSize);
    }

    public class SizeRangeFactory : ISizeRangeFactory
    {
        public ISizeRange Build() => NameSizeRange.All;
        public ISizeRange Build(NameSizeRange.Names name) => new NameSizeRange(name);
        public ISizeRange Build(NameSizeRange.Names minName, NameSizeRange.Names maxName) => new NameSizeRange(minName, maxName);
        public ISizeRange Build(Size size) => Build(size);
        public ISizeRange Build(Size minSize, Size maxSize)
        {
            if (minSize > maxSize)
            {
                (minSize, maxSize) = (maxSize, minSize);
            }

            if (minSize == Size.MinValue && maxSize == Size.MaxValue)
            {
                return NameSizeRange.All;
            }

            NameSizeRange.Names minName = NameSizeRange.Names.Empty;
            NameSizeRange.Names maxName = NameSizeRange.Names.Huge;

            var names = Enum.GetValues(typeof(NameSizeRange.Names)).Cast<NameSizeRange.Names>();
            foreach (var name in names)
            {
                var range = new NameSizeRange(name, name);
                if (range.MinSize == minSize)
                {
                    minName = range.MinName;
                }
                if (range.MaxSize == maxSize)
                {
                    maxName = range.MaxName;
                }
            }

            if (minName != NameSizeRange.Names.Empty || maxName != NameSizeRange.Names.Huge)
            {
                return new NameSizeRange(minName, maxName);
            }
            return new SizeRange(minSize, maxSize);
        }
    }

    public struct SizeRange : ISizeRange
    {
        public Size MinSize { get; }
        public Size MaxSize { get; }

        public SizeRange(Size size) => (MinSize, MaxSize) = (size, size);
        public SizeRange(Size minSize, Size maxSize) => (MinSize, MaxSize) = (minSize, maxSize);

        public void Deconstruct(out Size minSize, out Size maxSize) => (minSize, maxSize) = (MinSize, MaxSize);

        public override int GetHashCode() => (MinSize, MaxSize).GetHashCode();
        public override bool Equals(object other) => other is ISizeRange range && Equals(range);
        public bool Equals(ISizeRange other) => other is SizeRange range && range.MinSize == MinSize && range.MaxSize == MaxSize;

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

            if (MinSize == MaxSize)
            {
                return $"{MinSize}";
            }

            bool hasMin = MinSize != Size.MinValue && MinSize != Size.MaxValue;
            bool hasMax = MaxSize != Size.MinValue && MaxSize != Size.MaxValue;

            return string.Format(GetFormat(), MinSize, MaxSize);

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

    public struct NameSizeRange : ISizeRange
    {
        public enum Names : ushort { Empty, Tiny, Small, Medium, Large, VeryLarge, Huge }

        public static NameSizeRange All => new NameSizeRange(Names.Empty, Names.Huge);

        public Names MinName { get; }
        public Names MaxName { get; }

        public Size MinSize => MinName switch
        {
            Names.Empty => new Size(0),
            Names.Tiny => new Size(1),
            Names.Small => new Size(16, Size.Units.Kibi),
            Names.Medium => new Size(1, Size.Units.Mebi),
            Names.Large => new Size(128, Size.Units.Mebi),
            Names.VeryLarge => new Size(1, Size.Units.Gibi),
            Names.Huge => new Size(5, Size.Units.Gibi),
            _ => throw new ArgumentException(),
        };
        public Size MaxSize => MaxName switch
        {
            Names.Empty => new Size(0),
            Names.Tiny => new Size(16, Size.Units.Kibi),
            Names.Small => new Size(1, Size.Units.Mebi),
            Names.Medium => new Size(128, Size.Units.Mebi),
            Names.Large => new Size(1, Size.Units.Gibi),
            Names.VeryLarge => new Size(5, Size.Units.Gibi),
            Names.Huge => Size.MaxValue,
            _ => throw new ArgumentException(),
        };

        public NameSizeRange(Names name) => (MinName, MaxName) = (name, name);
        public NameSizeRange(Names minName, Names maxName) => (MinName, MaxName) = (minName, maxName);

        public bool Equals(ISizeRange other) => other is NameSizeRange range && range.MinName == MinName && range.MaxName == MaxName;

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == "r" || format == "R")
            {
                return new SizeRange(MinSize, MaxSize).ToString(format, formatProvider);
            }

            bool hasMin = MinName != Names.Empty && MinName != Names.Huge;
            bool hasMax = MaxName != Names.Empty && MaxName != Names.Huge;

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
                Names.Empty => "Empty",
                Names.Tiny => "ItemSizeText_Tiny".GetLocalized(),
                Names.Small => "ItemSizeText_Small".GetLocalized(),
                Names.Medium => "ItemSizeText_Medium".GetLocalized(),
                Names.Large => "ItemSizeText_Large".GetLocalized(),
                Names.VeryLarge => "ItemSizeText_VeryLarge".GetLocalized(),
                Names.Huge => "ItemSizeText_Huge".GetLocalized(),
                _ => null,
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
