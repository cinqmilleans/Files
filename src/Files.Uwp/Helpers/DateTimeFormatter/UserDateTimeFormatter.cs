using Files.Shared.Enums;
using System;
using Windows.Storage;

namespace Files.Uwp.Helpers
{
    internal class UserDateTimeFormatter : IDateTimeFormatter
    {
        private readonly IDateTimeFormatterFactory factory = new DateTimeFormatterFactory();

        private IDateTimeFormatter formatter;

        public string Name => formatter.Name;

        public UserDateTimeFormatter() => Update();

        public string ToShortLabel(DateTimeOffset offset) => formatter.ToShortLabel(offset);
        public string ToLongLabel(DateTimeOffset offset) => formatter.ToLongLabel(offset);

        public ITimeSpanLabel ToTimeSpanLabel(DateTimeOffset offset) => formatter.ToTimeSpanLabel(offset);

        private void Update()
        {
            var settings = ApplicationData.Current.LocalSettings;
            var timeStyle = Enum.Parse<TimeStyle>(settings.Values[Constants.LocalSettings.DateTimeFormat].ToString());

            formatter = factory.GetDateTimeFormatter(timeStyle);
        }
    }
}
