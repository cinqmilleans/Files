using ByteSizeLib;
using Files.Extensions;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public class SizeRange : IEquatable<SizeRange>, IFormattable
    {
        private static readonly Lazy<SizeRange> all = new Lazy<SizeRange>(() => new SizeRange(new AllRange()));
        private static readonly Lazy<SizeRange> empty = new Lazy<SizeRange>(() => new SizeRange(new EmptyRange()));
        private static readonly Lazy<SizeRange> tiny = new Lazy<SizeRange>(() => new SizeRange(new TinyRange()));
        private static readonly Lazy<SizeRange> small = new Lazy<SizeRange>(() => new SizeRange(new SmallRange()));
        private static readonly Lazy<SizeRange> medium = new Lazy<SizeRange>(() => new SizeRange(new MediumRange()));
        private static readonly Lazy<SizeRange> large = new Lazy<SizeRange>(() => new SizeRange(new LargeRange()));
        private static readonly Lazy<SizeRange> veryLarge = new Lazy<SizeRange>(() => new SizeRange(new VeryLargeRange()));
        private static readonly Lazy<SizeRange> huge = new Lazy<SizeRange>(() => new SizeRange(new HugeRange()));

        private readonly Range range;

        public static SizeRange All => all.Value;
        public static SizeRange Empty => empty.Value;
        public static SizeRange Tiny => tiny.Value;
        public static SizeRange Small => small.Value;
        public static SizeRange Medium => medium.Value;
        public static SizeRange Large => large.Value;
        public static SizeRange VeryLarge => veryLarge.Value;
        public static SizeRange Huge => huge.Value;

        public static IEnumerable<SizeRange> NamedRanges
        {
            get
            {
                yield return All;
                yield return Empty;
                yield return Tiny;
                yield return Small;
                yield return Medium;
                yield return Large;
                yield return VeryLarge;
                yield return Huge;
            }
        }

        public Size MinSize => range.MinSize;
        public Size MaxSize => range.MaxSize;

        public SizeRange() : this(new AllRange()) { }
        public SizeRange(Size minSize, Size maxSize) => range = GetRange(minSize, maxSize);
        private SizeRange(Range range) => this.range = range;

        public void Deconstruct(out Size minSize, out Size maxSize) => (minSize, maxSize) = range;

        public override int GetHashCode() => range.GetHashCode();
        public override bool Equals(object other) => range.Equals(other);
        public virtual bool Equals(SizeRange other) => range.Equals(other.range);

        public override string ToString() => range.ToString();
        public string ToString(string format) => range.ToString(format);
        public virtual string ToString(string format, IFormatProvider formatProvider) => range.ToString(format, formatProvider);

        private static Range GetRange(Size minSize, Size maxSize)
        {
            if (minSize > maxSize)
            {
                (minSize, maxSize) = (maxSize, minSize);
            }

            var named = NamedRanges.FirstOrDefault(range => (range.MinSize, range.MaxSize).Equals((minSize, maxSize)));
            if (named is not null)
            {
                return named.range;
            }

            if (minSize.Equals(Size.MaxValue))
            {
                return new MaxRange();
            }
            if (minSize.Equals(maxSize))
            {
                return new EqualRange(minSize);
            }
            if (minSize.Equals(Size.MinValue))
            {
                return new LessRange(maxSize);
            }
            if (maxSize.Equals(Size.MaxValue))
            {
                return new GreaterRange(minSize);
            }
            return new BetweenRange(minSize, maxSize);
        }

        private abstract class Range : IEquatable<Range>, IFormattable
        {
            public virtual Size MinSize { get; } = Size.MinValue;
            public virtual Size MaxSize { get; } = Size.MaxValue;

            public virtual string Name => null;
            public virtual string ShortFormat => string.Empty;
            public virtual string FullFormat => ShortFormat;

            public void Deconstruct(out Size minSize, out Size maxSize)
                => (minSize, maxSize) = (MinSize, MaxSize);

            public override int GetHashCode() => (MinSize, MaxSize).GetHashCode();
            public override bool Equals(object other) => other is Range range && Equals(range);
            public bool Equals(Range other) => (other.MinSize, other.MaxSize).Equals((MinSize, MaxSize));

            public override string ToString() => ToString("G");
            public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
            public string ToString(string format, IFormatProvider formatProvider) => format switch
            {
                "G" => ToString("N", formatProvider),
                "n" => Name ?? ToString("r", formatProvider),
                "N" => Name ?? ToString("R", formatProvider),
                "r" => string.Format(ShortFormat, MinSize, MaxSize),
                "R" => string.Format(FullFormat, MinSize, MaxSize),
                _ => string.Empty,
            };
        }
        private class AllRange : Range
        {
            public override string Name => string.Empty;
        }
        private class EmptyRange : EqualRange
        {
            public override string Name => "Empty";
            public EmptyRange() : base(new Size(0)) {}
        }
        private class TinyRange : LessRange
        {
            public override string Name => "ItemSizeText_Tiny".GetLocalized();
            public TinyRange() : base(new Size(16, Size.Units.Kibi)) {}
        }
        private class SmallRange : BetweenRange
        {
            public override string Name => "ItemSizeText_Small".GetLocalized();
            public SmallRange() : base(new Size(16, Size.Units.Kibi), new Size(1, Size.Units.Mebi)) {}
        }
        private class MediumRange : BetweenRange
        {
            public override string Name => "ItemSizeText_Medium".GetLocalized();
            public MediumRange() : base(new Size(1, Size.Units.Mebi), new Size(128, Size.Units.Mebi)) {}
        }
        private class LargeRange : BetweenRange
        {
            public override string Name => "ItemSizeText_Large".GetLocalized();
            public LargeRange() : base(new Size(128, Size.Units.Mebi), new Size(1, Size.Units.Gibi)) {}
        }
        private class VeryLargeRange : BetweenRange
        {
            public override string Name => "ItemSizeText_VeryLarge".GetLocalized();
            public VeryLargeRange() : base(new Size(1, Size.Units.Gibi), new Size(5, Size.Units.Gibi)) {}
        }
        private class HugeRange : GreaterRange
        {
            public override string Name => "ItemSizeText_Huge".GetLocalized();
            public HugeRange() : base(new Size(5, Size.Units.Gibi)) {}
        }
        private class MaxRange : Range
        {
            public override Size MinSize { get; } = Size.MaxValue;
        }
        private class EqualRange : Range
        {
            public override Size MinSize { get; }
            public override Size MaxSize => MinSize;
            public override string ShortFormat => "{1}";
            public EqualRange(Size size) => MinSize = size;
        }
        private class LessRange : Range
        {
            public override Size MaxSize { get; }
            public override string ShortFormat => "< {1}";
            public override string FullFormat => "Less than {1}";
            public LessRange(Size maxSize) => MaxSize = maxSize;
        }
        private class GreaterRange : Range
        {
            public override Size MinSize { get; }
            public override string ShortFormat => "> {0}";
            public override string FullFormat => "Greater than {0}";
            public GreaterRange(Size minSize) => MinSize = minSize;
        }
        private class BetweenRange : Range
        {
            public override Size MinSize { get; }
            public override Size MaxSize { get; }
            public override string ShortFormat => "{0} - {1}";
            public override string FullFormat => "Between {0} and {1}";
            public BetweenRange(Size minSize, Size maxSize) => (MinSize, MaxSize) = (minSize, maxSize);
        }
    }

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
}
