using Files.Extensions;
using System;
using System.Globalization;
using Lib = ByteSizeLib;

namespace Files.BackEnd
{
    public enum ByteSizeUnits : ushort
    {
        Byte,
        Kibi,
        Mebi,
        Gibi,
        Tebi,
        Pebi,
    }

    public struct ByteSize : IEquatable<ByteSize>, IComparable<ByteSize>, IFormattable
    {
        private readonly Lib.ByteSize size;

        public static readonly ByteSize MinValue = new(0);
        public static readonly ByteSize MaxValue = new(long.MaxValue);

        public ulong Bytes => (ulong)size.Bits * Lib.ByteSize.BitsInByte;

        public double Value => size.LargestWholeNumberBinaryValue;
        public ByteSizeUnits Unit => size.LargestWholeNumberBinarySymbol switch
        {
            Lib.ByteSize.ByteSymbol => ByteSizeUnits.Byte,
            Lib.ByteSize.KibiByteSymbol => ByteSizeUnits.Kibi,
            Lib.ByteSize.MebiByteSymbol => ByteSizeUnits.Mebi,
            Lib.ByteSize.GibiByteSymbol => ByteSizeUnits.Gibi,
            Lib.ByteSize.TebiByteSymbol => ByteSizeUnits.Tebi,
            Lib.ByteSize.PebiByteSymbol => ByteSizeUnits.Pebi,
            _ => throw new InvalidOperationException(),
        };

        public ByteSize(ulong bytes)
        {
            if (bytes < 0)
            {
                throw new ArgumentException("Size is always positive.");
            }
            if (bytes > long.MaxValue)
            {
                throw new ArgumentException("The maximum size is long.MaxValue.");
            }
            size = Lib.ByteSize.FromBytes((long)bytes);
        }
        public ByteSize(double value, ByteSizeUnits unit)
        {
            if (value < 0)
            {
                throw new ArgumentException("Size is always positive.");
            }
            size = unit switch
            {
                ByteSizeUnits.Byte => Lib.ByteSize.FromBytes(value),
                ByteSizeUnits.Kibi => Lib.ByteSize.FromKibiBytes(value),
                ByteSizeUnits.Mebi => Lib.ByteSize.FromMebiBytes(value),
                ByteSizeUnits.Gibi => Lib.ByteSize.FromGibiBytes(value),
                ByteSizeUnits.Tebi => Lib.ByteSize.FromTebiBytes(value),
                ByteSizeUnits.Pebi => Lib.ByteSize.FromPebiBytes(value),
                _ => throw new ArgumentException(),
            };
        }

        public static explicit operator long(ByteSize size) => (long)size.Bytes;
        public static explicit operator ulong(ByteSize size) => size.Bytes;
        public static implicit operator ByteSize(long size) => new((ulong)size);
        public static implicit operator ByteSize(ulong size) => new(size);

        public static ByteSize operator +(ByteSize a, ByteSize b) => new(a.Bytes + b.Bytes);
        public static ByteSize operator -(ByteSize a, ByteSize b) => new(a.Bytes - b.Bytes);
        public static bool operator ==(ByteSize a, ByteSize b) => a.size == b.size;
        public static bool operator !=(ByteSize a, ByteSize b) => a.size != b.size;
        public static bool operator <(ByteSize a, ByteSize b) => a.size < b.size;
        public static bool operator >(ByteSize a, ByteSize b) => a.size > b.size;
        public static bool operator <=(ByteSize a, ByteSize b) => a.size <= b.size;
        public static bool operator >=(ByteSize a, ByteSize b) => a.size >= b.size;

        public override int GetHashCode() => size.GetHashCode();
        public override bool Equals(object other) => other is ByteSize size && Equals(size);
        public bool Equals(ByteSize other) => other.size.Equals(size);
        public int CompareTo(ByteSize other) => other.size.CompareTo(size);

        public override string ToString() => ToString("G");
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);
        public string ToString(string format, IFormatProvider formatProvider) => (format ?? "G").ToLower() switch
        {
            "b" => Bytes.ToString(),
            "B" => Bytes.ToString(),
            "g" => size.ToSizeString(),
            "G" => size.ToSizeString(),
            "l" => size.ToLongSizeString(),
            "L" => size.ToLongSizeString(),
            "u" => size.LargestWholeNumberBinarySymbol.ConvertSizeAbbreviation(),
            "U" => size.LargestWholeNumberBinarySymbol.ConvertSizeAbbreviation(),
            _ => string.Empty,
        };
    }
}
