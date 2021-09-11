using ByteSizeLib;
using Files.Extensions;
using Microsoft.Toolkit.Uwp;
using System;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public struct Size : IEquatable<Size>, IComparable<Size>
    {
        public enum Units { Byte, Kibi, Mebi, Gibi, Tebi, Pebi }

        private readonly ByteSize size;

        public static Size MinValue { get; } = new Size(0);
        public static Size MaxValue { get; } = new Size(ByteSize.MaxValue);

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

        public override string ToString() => size.ToBinaryString().ConvertSizeAbbreviation();
    }

    public class SizeRange : IEquatable<SizeRange>, IFormattable
    {
        public Size MinSize { get; }
        public Size MaxSize { get; }

        public SizeRange() : this(Size.MinValue, Size.MaxValue) {}
        public SizeRange(Size minSize, Size maxSize) => (MinSize, MaxSize) = minSize <= maxSize ? (minSize, maxSize) : (maxSize, minSize);
        protected SizeRange((Size minSize, Size maxSize) range) : this(range.minSize, range.maxSize) {}

        public void Deconstruct(out Size minSize, out Size maxSize) => (minSize, maxSize) = (MinSize, MaxSize);

        public override int GetHashCode() => (MinSize, MaxSize).GetHashCode();
        public override bool Equals(object other) => other is SizeRange range && Equals(range);
        public virtual bool Equals(SizeRange other) => (other.MinSize, other.MaxSize).Equals((MinSize, MaxSize));

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            bool hasMinSize = MinSize != Size.MinValue && MinSize != Size.MaxValue;
            bool hasMaxSize = MaxSize != Size.MinValue && MaxSize != Size.MaxValue;

            return format switch
            {
                "r" => string.Format(GetShortFormat(), MinSize, MaxSize),
                "R" => string.Format(GetFullFormat(), MinSize, MaxSize),
                _ => string.Empty,
            };

            string GetShortFormat() => (hasMinSize, hasMaxSize) switch
            {
                (false, false) => string.Empty,
                (true, false) => "< {1}",
                (false, true) => "> {1}",
                _ when MinSize == MaxSize => "{1}",
                _ => "{0} - {1}",
            };
            string GetFullFormat() => (hasMinSize, hasMaxSize) switch
            {
                (false, false) => string.Empty,
                (false, true) => "Less than {1}",
                (true, false) => "Greater than {0}",
                _ when MinSize == MaxSize => "{1}",
                _ => "Between {0} and {1}",
            };
        }
    }

    public class NamedSizeRange : SizeRange, IEquatable<NamedSizeRange>
    {
        public enum Names { Unnamed, All, Empty, Tiny, Small, Medium, Large, VeryLarge, Huge }

        public Names Name { get; }

        public NamedSizeRange() : this(Names.All) { }
        public NamedSizeRange(Names name) : base(GetRange(name)) => Name = name;
        public NamedSizeRange(Size minSize, Size maxSize) : base(minSize, maxSize) => Name = GetName(minSize, maxSize);

        public void Deconstruct(out Names name, out Size minSize, out Size maxSize) => (name, minSize, maxSize) = (Name, MinSize, MaxSize);

        public override int GetHashCode() => (Name, MinSize, MaxSize).GetHashCode();
        public override bool Equals(object other) => other switch
        {
            NamedSizeRange range => Equals(range),
            SizeRange range => Equals(range),
            _ => false,
        };
        public virtual bool Equals(NamedSizeRange other) => (other.Name, other.MinSize, other.MaxSize).Equals((Name, MinSize, MaxSize));

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
                Names.All => string.Empty,
                Names.Empty => "Empty",
                Names.Tiny => "ItemSizeText_Tiny".GetLocalized(),
                Names.Small => "ItemSizeText_Small".GetLocalized(),
                Names.Medium => "ItemSizeText_Medium".GetLocalized(),
                Names.Large => "ItemSizeText_Large".GetLocalized(),
                Names.VeryLarge => "ItemSizeText_VeryLarge".GetLocalized(),
                Names.Huge => "ItemSizeText_Huge".GetLocalized(),
                _ => null,
            };
        }

        private static Names GetName(Size minSize, Size maxSize)
        {
            var names = Enum.GetValues(typeof(Names)).Cast<Names>().Where(name => name != Names.Unnamed);
            foreach (var name in names)
            {
                var range = new NamedSizeRange(name);
                if ((range.MinSize, range.MaxSize).Equals((minSize, maxSize)))
                {
                    return name;
                }
            }
            return Names.Unnamed;
        }

        private static (Size, Size) GetRange(Names name) => name switch
        {
            Names.All => (Size.MinValue, Size.MaxValue),
            Names.Empty => (new Size(0), new Size(0)),
            Names.Tiny => (new Size(0), new Size(16, Size.Units.Kibi)),
            Names.Small => (new Size(16, Size.Units.Kibi), new Size(1, Size.Units.Mebi)),
            Names.Medium => (new Size(1, Size.Units.Mebi), new Size(128, Size.Units.Mebi)),
            Names.Large => (new Size(128, Size.Units.Mebi), new Size(1, Size.Units.Gibi)),
            Names.VeryLarge => (new Size(1, Size.Units.Gibi), new Size(5, Size.Units.Gibi)),
            Names.Huge => (new Size(5, Size.Units.Gibi), Size.MaxValue),
            _ => throw new ArgumentException(),
        };
    }
}
