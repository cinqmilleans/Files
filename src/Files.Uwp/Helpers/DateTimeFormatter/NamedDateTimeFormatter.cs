using System;

namespace Files.Uwp.Helpers
{
    internal class NamedDateTimeFormatter : INamedDateTimeFormatter
    {
        private readonly IDateTimeFormatter formatter;

        public string Name { get; }

        public NamedDateTimeFormatter(string name, IDateTimeFormatter formatter)
            => (Name, this.formatter) = (name, formatter);

        public string ToShortLabel(DateTimeOffset offset) => formatter.ToShortLabel(offset);
        public string ToLongLabel(DateTimeOffset offset) => formatter.ToLongLabel(offset);

        public ITimeSpanLabel ToTimeSpanLabel(DateTimeOffset offset) => formatter.ToTimeSpanLabel(offset);
    }
}
