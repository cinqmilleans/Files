using System;

namespace Files.Article.Extension
{
    internal static class Temp
    {
        public static string GetLocalized(this string key) => key;

        public static string ToSizeString(this long size) => size.ToString();
        public static string ToLongSizeString(this long size) => size.ToString();

        public static string GetFriendlyDateFromFormat(this DateTimeOffset date, string format)
            => date.ToString(format);
    }
}
