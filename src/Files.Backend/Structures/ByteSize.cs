using Files.Backend.Extensions;
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

        public static readonly ByteSize Zero = new(0L);
        public static readonly ByteSize MaxValue = new(long.MaxValue);

        public ulong Bytes => (ulong)size.Bytes;

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
            size = bytes switch
            {
                > long.MaxValue => throw new ArgumentException($"The maximum size is {long.MaxValue}."),
                _ => Lib.ByteSize.FromBytes((long)bytes),
            };
        }
        public ByteSize(double value, ByteSizeUnits unit)
        {
            if (value < 0)
            {
                throw new ArgumentException("Size is always positive.", nameof(value));
            }
            size = unit switch
            {
                ByteSizeUnits.Byte => Lib.ByteSize.FromBytes(value),
                ByteSizeUnits.Kibi => Lib.ByteSize.FromKibiBytes(value),
                ByteSizeUnits.Mebi => Lib.ByteSize.FromMebiBytes(value),
                ByteSizeUnits.Gibi => Lib.ByteSize.FromGibiBytes(value),
                ByteSizeUnits.Tebi => Lib.ByteSize.FromTebiBytes(value),
                ByteSizeUnits.Pebi => Lib.ByteSize.FromPebiBytes(value),
                _ => throw new ArgumentException("Invalid byte size unit", nameof(unit)),
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


        public string ToShortString() => ToString("G");
        public string ToLongString() => ToString("L");

        public override string ToString() => ToString("G");

        /// <summary>Converts this to a formatted string.</summary>
        /// <param name="format">
        ///   A format string. This may have the following values:
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Format strings</term>
        ///     </listheader>
        ///     <item>
        ///       <term>"b"</term>
        ///       <description>Format bytes value.</description>
        ///     </item>
        ///     <item>
        ///       <term>"v"</term>
        ///       <description>Format numeric value</description>
        ///     </item>
        ///     <item>
        ///       <term>"u"</term>
        ///       <description>Format to unit</description>
        ///     </item>
        ///     <item>
        ///       <term>"g"</term>
        ///       <description>Format to short string</description>
        ///     </item>
        ///     <item>
        ///       <term>"l"</term>
        ///       <description>Format to long string</description>
        ///     </item>
        ///   </list>
        /// </param>
        /// <returns>The formatted string.</returns>
        public string ToString(string format) => ToString(format, CultureInfo.CurrentCulture);

        /// <summary>Converts this to a formatted string.</summary>
        /// <param name="format">
        ///   A format string. This may have the following values:
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Format strings</term>
        ///     </listheader>
        ///     <item>
        ///       <term>"b"</term>
        ///       <description>Format bytes value.</description>
        ///     </item>
        ///     <item>
        ///       <term>"v"</term>
        ///       <description>Format numeric value</description>
        ///     </item>
        ///     <item>
        ///       <term>"u"</term>
        ///       <description>Format to unit</description>
        ///     </item>
        ///     <item>
        ///       <term>"g"</term>
        ///       <description>Format to short string</description>
        ///     </item>
        ///     <item>
        ///       <term>"l"</term>
        ///       <description>Format to long string</description>
        ///     </item>
        ///   </list>
        /// </param>
        /// <param name="formatProvider">A format provider.</param>
        /// <returns>The formatted string.</returns>
        public string ToString(string format, IFormatProvider formatProvider) => format switch
        {
            "b" or "B" => Bytes.ToString(),
            "v" or "V" => $"{Value:0.##}",
            "g" or "G" => $"{Value:0.##} {ToLocalizedUnit()}",
            "l" or "L" => $"{Value:0.##} {ToLocalizedUnit()} ({size.Bytes:#,##0} {"ItemSizeBytes".ToLocalized()})",
            "u" or "U" => ToLocalizedUnit(),
            _ => Value.ToString("format"),
        };

        private string ToLocalizedUnit() => Unit switch
        {
            ByteSizeUnits.Byte => "ByteSymbol".ToLocalized(),
            ByteSizeUnits.Kibi => "KiloByteSymbol".ToLocalized(),
            ByteSizeUnits.Mebi => "MegaByteSymbol".ToLocalized(),
            ByteSizeUnits.Gibi => "GigaByteSymbol".ToLocalized(),
            ByteSizeUnits.Tebi => "TeraByteSymbol".ToLocalized(),
            ByteSizeUnits.Pebi => "PetaByteSymbol".ToLocalized(),
            _ => throw new InvalidOperationException(),
        };
    }
}
