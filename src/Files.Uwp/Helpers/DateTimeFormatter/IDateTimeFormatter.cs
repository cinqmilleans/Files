using System;

namespace Files.Uwp.Helpers
{
    public interface IDateTimeFormatter
    {
        string Name { get; }

        string ToShortLabel(DateTimeOffset offset);
        string ToLongLabel(DateTimeOffset offset);

        ITimeSpanLabel ToTimeSpanLabel(DateTimeOffset offset);
    }
}
