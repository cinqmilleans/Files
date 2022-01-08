using ByteSizeLib;
using Files.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISizeRangeFilter : ISearchFilter
    {
        SizeRange Range { get; set; }
    }

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
        public static readonly SizeRange None = new(new NoneRange());
        public static readonly SizeRange All = new(new AllRange());
        public static readonly SizeRange Empty = new(new NamedRange(NamedRange.Names.Empty));
        public static readonly SizeRange Tiny = new(new NamedRange(NamedRange.Names.Tiny));
        public static readonly SizeRange Small = new(new NamedRange(NamedRange.Names.Small));
        public static readonly SizeRange Medium = new(new NamedRange(NamedRange.Names.Medium));
        public static readonly SizeRange Large = new(new NamedRange(NamedRange.Names.Large));
        public static readonly SizeRange VeryLarge = new(new NamedRange(NamedRange.Names.VeryLarge));
        public static readonly SizeRange Huge = new(new NamedRange(NamedRange.Names.Huge));

        private readonly IRange range;

        public bool IsNamed => range is not CustomRange;

        public RangeDirections Direction => range.Direction;

        public Size MinValue => range.MinValue;
        public Size MaxValue => range.MaxValue;

        public IRange<string> Label => range.Label;

        public SizeRange(Size minSize, Size maxSize) => range = GetRange(minSize, maxSize);
        private SizeRange(IRange range) => this.range = range;

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

        public static SizeRange operator +(SizeRange a, SizeRange b)
        {
            Size minSize = a.MinValue <= b.MinValue ? a.MinValue : b.MinValue;
            Size maxSize = b.MaxValue >= a.MaxValue ? b.MaxValue : a.MaxValue;

            return new(minSize, maxSize);
        }
        public static SizeRange operator -(SizeRange a, SizeRange b)
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
        public static bool operator ==(SizeRange a, SizeRange b) => a.Equals(b);
        public static bool operator !=(SizeRange a, SizeRange b) => !a.Equals(b);
        public static bool operator <(SizeRange a, SizeRange b) => a.MaxValue < b.MinValue;
        public static bool operator >(SizeRange a, SizeRange b) => a.MaxValue > b.MinValue;
        public static bool operator <=(SizeRange a, SizeRange b) => a.MaxValue <= b.MinValue;
        public static bool operator >=(SizeRange a, SizeRange b) => a.MaxValue >= b.MinValue;

        public bool Contains(Size size) => size >= MinValue && size <= MaxValue;
        public bool Contains(IRange<Size> range) => range.MinValue >= MinValue && range.MaxValue <= MaxValue;

        private static IRange GetRange(Size minSize, Size maxSize)
        {
            if (minSize > maxSize)
            {
                (minSize, maxSize) = (maxSize, minSize);
            }

            if (minSize == Size.MinValue && maxSize == Size.MaxValue)
            {
                return new AllRange();
            }

            var namedSizes = NamedRange.NamedSizes.ToList();

            bool hasMinName = namedSizes.Any(namedSize => namedSize.minSize == minSize);
            bool hasMaxName = namedSizes.Any(namedSize => namedSize.maxSize == maxSize);

            if (!hasMinName || !hasMaxName)
            {
                return new CustomRange(minSize, maxSize);
            }

            NamedRange.Names minName = namedSizes.First(namedSize => namedSize.minSize == minSize).name;
            NamedRange.Names maxName = namedSizes.First(namedSize => namedSize.maxSize == maxSize).name;

            return new NamedRange(minName, maxName);
        }

        private interface IRange : IRange<Size>, IEquatable<IRange<Size>>
        {
        }

        private struct NoneRange : IRange
        {
            public RangeDirections Direction => RangeDirections.None;

            public Size MinValue => Size.MaxValue;
            public Size MaxValue => Size.MinValue;

            public IRange<string> Label => RangeLabel.None;

            public override int GetHashCode() => Direction.GetHashCode();
            public override bool Equals(object other) => other is NoneRange;
            public bool Equals(IRange<Size> other) => other is NoneRange;
        }

        private struct AllRange : IRange
        {
            public RangeDirections Direction => RangeDirections.None;

            public Size MinValue => Size.MinValue;
            public Size MaxValue => Size.MaxValue;

            public IRange<string> Label => RangeLabel.None;

            public override int GetHashCode() => Direction.GetHashCode();
            public override bool Equals(object other) => other is AllRange;
            public bool Equals(IRange<Size> other) => other is AllRange;
        }

        private struct NamedRange : IRange
        {
            public enum Names { Empty, Tiny, Small, Medium, Large, VeryLarge, Huge }

            public static IEnumerable<(Names name, Size minSize, Size maxSize)> NamedSizes
                => Enum.GetValues(typeof(Names)).Cast<Names>()
                    .Select(name => (name, GetMinSize(name), GetMaxSize(name)));

            private readonly Names minName;
            private readonly Names maxName;

            public RangeDirections Direction { get; }

            public Size MinValue => GetMinSize(minName);
            public Size MaxValue => GetMaxSize(minName);

            public IRange<string> Label => new RangeLabel(GetText(minName), GetText(maxName));

            public NamedRange(Names name) : this(name, name) {}
            public NamedRange(Names minName, Names maxName)
            {
                (this.minName, this.maxName) = (minName, maxName);

                Direction = (minName, maxName) switch
                {
                    (Names.Empty, _) => RangeDirections.LessThan,
                    (_, Names.Huge) => RangeDirections.GreaterThan,
                    _ when minName == maxName => RangeDirections.EqualTo,
                    _ => RangeDirections.Between,
                };
            }

            public override int GetHashCode()
                => (minName, maxName).GetHashCode();
            public override bool Equals(object other)
                => other is NamedRange range && Equals(range);
            public bool Equals(IRange<Size> other)
                => other is NamedRange range && range.minName == minName && range.maxName == maxName;

            private static Size GetMinSize(Names name) => name switch
            {
                Names.Empty => Size.MinValue,
                Names.Tiny => new Size(1),
                Names.Small => new Size(16, Size.Units.Kibi),
                Names.Medium => new Size(1, Size.Units.Mebi),
                Names.Large => new Size(128, Size.Units.Mebi),
                Names.VeryLarge => new Size(1, Size.Units.Gibi),
                Names.Huge => new Size(5, Size.Units.Gibi),
                _ => throw new ArgumentException(),
            };
            private static Size GetMaxSize(Names name) => name switch
            {
                Names.Empty => Size.MinValue,
                Names.Tiny => new Size(16, Size.Units.Kibi),
                Names.Small => new Size(1, Size.Units.Mebi),
                Names.Medium => new Size(128, Size.Units.Mebi),
                Names.Large => new Size(1, Size.Units.Gibi),
                Names.VeryLarge => new Size(5, Size.Units.Gibi),
                Names.Huge => Size.MaxValue,
                _ => throw new ArgumentException(),
            };
            private static string GetText(Names name) => name switch
            {
                Names.Empty => "ItemSizeText_Empty".GetLocalized(),
                Names.Tiny => "ItemSizeText_Tiny".GetLocalized(),
                Names.Small => "ItemSizeText_Small".GetLocalized(),
                Names.Medium => "ItemSizeText_Medium".GetLocalized(),
                Names.Large => "ItemSizeText_Large".GetLocalized(),
                Names.VeryLarge => "ItemSizeText_Large".GetLocalized(),
                Names.Huge => "ItemSizeText_Huge".GetLocalized(),
                _ => throw new ArgumentException(),
            };
        }

        private struct CustomRange : IRange
        {
            public RangeDirections Direction { get; }

            public Size MinValue { get; }
            public Size MaxValue { get; }

            public IRange<string> Label => new RangeLabel($"{MinValue}", $"{MaxValue}");

            public CustomRange(Size minSize, Size maxSize)
            {
                MinValue = minSize <= maxSize ? minSize : maxSize;
                MaxValue = maxSize >= minSize ? maxSize : minSize;

                bool hasMin = MinValue > Size.MinValue;
                bool hasMax = MaxValue < Size.MaxValue;

                Direction = (hasMin, hasMax) switch
                {
                    (false, false) => RangeDirections.None,
                    _ when MinValue == MaxValue => RangeDirections.EqualTo,
                    (true, false) => RangeDirections.GreaterThan,
                    (false, true) => RangeDirections.LessThan,
                    _ => RangeDirections.Between,
                };
            }

            public override int GetHashCode()
                => (MinValue, MaxValue).GetHashCode();
            public override bool Equals(object other)
                => other is CustomRange range && Equals(range);
            public bool Equals(IRange<Size> other)
                => other is CustomRange range && range.MinValue == MinValue && range.MaxValue == MaxValue;
        }
    }

    [SearchHeader]
    public class SizeHeader : ISearchHeader
    {
        public string Key => "size";
        public string Glyph => "\uE2B2";
        public string Label => "Size".GetLocalized();
        public string Description => string.Empty;

        ISearchFilter ISearchHeader.GetFilter() => GetFilter();
        public ISizeRangeFilter GetFilter() => new SizeRangeFilter();
    }

    public class SizeRangeFilter : ObservableObject, ISizeRangeFilter
    {
        public ISearchHeader Header { get; } = new SizeHeader();

        public bool IsEmpty => range == SizeRange.None || range == SizeRange.All;

        private SizeRange range = SizeRange.All;
        public SizeRange Range
        {
            get => range;
            set
            {
                if (SetProperty(ref range, value))
                {
                    OnPropertyChanged(nameof(IsEmpty));
                    OnPropertyChanged(nameof(Tags));
                }
            }
        }

        public IEnumerable<ISearchTag> Tags => Range.Direction switch
        {
            RangeDirections.EqualTo => new EqualTag(this).CreateEnumerable(),
            RangeDirections.GreaterThan => new FromTag(this).CreateEnumerable(),
            RangeDirections.LessThan => new ToTag(this).CreateEnumerable(),
            RangeDirections.Between => new List<ISearchTag> { new FromTag(this), new ToTag(this) },
            _ => Enumerable.Empty<ISearchTag>(),
        };

        public SizeRangeFilter() {}
        public SizeRangeFilter(SizeRange range) => Range = range;

        public void Clear() => Range = SizeRange.All;

        public string ToAdvancedQuerySyntax()
        {
            var (direction, minValue, maxValue) = Range;

            return direction switch
            {
                RangeDirections.EqualTo => $"System.Size:={minValue.Bytes}",
                RangeDirections.LessThan => $"System.Size:<={maxValue.Bytes}",
                RangeDirections.GreaterThan => $"System.Size:>={minValue.Bytes}",
                RangeDirections.Between => $"System.Size:{minValue.Bytes}..{maxValue.Bytes}",
                _ => string.Empty,
            };
        }

        private class EqualTag : ISearchTag
        {
            ISearchFilter ISearchTag.Filter => Filter;
            public ISizeRangeFilter Filter { get; }

            public string Title => string.Empty;
            public string Parameter => Filter.Range.Label.MinValue;

            public EqualTag(ISizeRangeFilter filter) => Filter = filter;

            public void Delete() => Filter.Range = SizeRange.All;
        }
        private class FromTag : ISearchTag
        {
            ISearchFilter ISearchTag.Filter => Filter;
            public ISizeRangeFilter Filter { get; }

            public string Title => "Range_From".GetLocalized();
            public string Parameter => Filter.Range.Label.MinValue;

            public FromTag(ISizeRangeFilter filter) => Filter = filter;

            public void Delete() => Filter.Range = new SizeRange(Size.MinValue, Filter.Range.MaxValue);
        }
        private class ToTag : ISearchTag
        {
            ISearchFilter ISearchTag.Filter => Filter;
            public ISizeRangeFilter Filter { get; }

            public string Title => "Range_To".GetLocalized();
            public string Parameter => Filter.Range.Label.MinValue;

            public ToTag(ISizeRangeFilter filter) => Filter = filter;

            public void Delete() => Filter.Range = new SizeRange(Filter.Range.MinValue, Size.MaxValue);
        }
    }
}
