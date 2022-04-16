using System;

namespace Files.Uwp.Helpers
{
    public interface IDateTimeFormatter
    {
        string ToShortLabel(DateTimeOffset offset);
        string ToLongLabel(DateTimeOffset offset);

        ITimeSpanLabel ToTimeSpanLabel(DateTimeOffset offset);
    }
}
