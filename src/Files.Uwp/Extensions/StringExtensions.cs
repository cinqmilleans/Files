using ByteSizeLib;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Files.Uwp.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }

        private static readonly Dictionary<string, string> abbreviations = new Dictionary<string, string>()
        {
            { "KiB", "KiloByteSymbol".GetLocalized() },
            { "MiB", "MegaByteSymbol".GetLocalized() },
            { "GiB", "GigaByteSymbol".GetLocalized() },
            { "TiB", "TeraByteSymbol".GetLocalized() },
            { "PiB", "PetaByteSymbol".GetLocalized() },
            { "B", "ByteSymbol".GetLocalized() },
            { "b", "ByteSymbol".GetLocalized() }
        };

        public static string ConvertSizeAbbreviation(this string value)
        {
            foreach (var item in abbreviations)
            {
                value = value.Replace(item.Key, item.Value, StringComparison.Ordinal);
            }
            return value;
        }

        public static string ToSizeString(this long size) => ByteSize.FromBytes(size).ToSizeString();
        public static string ToSizeString(this ulong size) => ByteSize.FromBytes(size).ToSizeString();
        public static string ToSizeString(this ByteSize size) => size.ToBinaryString().ConvertSizeAbbreviation();

        public static string ToLongSizeString(this long size) => ByteSize.FromBytes(size).ToLongSizeString();
        public static string ToLongSizeString(this ulong size) => ByteSize.FromBytes(size).ToLongSizeString();
        public static string ToLongSizeString(this ByteSize size) => $"{size.ToSizeString()} ({size.Bytes:#,##0} {"ItemSizeBytes".GetLocalized()})";
    }
}
