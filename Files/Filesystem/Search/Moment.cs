using System;

namespace Files.Filesystem.Search
{
    internal enum Moment
    {
        Before,
        After,
        Between,
        DayAgo,
        WeekAgo,
        MonthAgo,
        YearAgo,
    }

    internal static class MomentExtension
    {
        public static string ToAdvancedSearchQueryFilter (this Moment moment)
        {
            return moment switch
            {
                Moment.DayAgo => string.Empty,
                Moment.WeekAgo => string.Empty,
                Moment.MonthAgo => string.Empty,
                Moment.YearAgo => string.Empty,
                _ => throw new ArgumentException()
            };
        }
    }
}
