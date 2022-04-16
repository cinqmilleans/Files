using System;

namespace Files.Uwp.Helpers
{
    internal class SystemDateTimeFormatter : AbstractDateTimeFormatter
    {
        public string DateFormat { get; }

        public SystemDateTimeFormatter(string dateFormat) => DateFormat = dateFormat;

        public override string ToShortLabel(DateTimeOffset offset)
        {
            if (offset.Year <= 1601 || offset.Year >= 9999)
            {
                return " ";
            }
            return offset.ToLocalTime().ToString(DateFormat);
        }
        public override string ToLongLabel(DateTimeOffset offset)
        {
            if (offset.Year <= 1601 || offset.Year >= 9999)
            {
                return " ";
            }
            return offset.ToLocalTime().ToString($"{DateFormat} t");
        }

        public override ITimeSpanLabel ToTimeSpanLabel(DateTimeOffset offset)
        {
            var label = base.ToTimeSpanLabel(offset);
            return new TimeSpanLabel(label.Range, label.Range, label.Glyph, label.Index);
        }
    }
}
