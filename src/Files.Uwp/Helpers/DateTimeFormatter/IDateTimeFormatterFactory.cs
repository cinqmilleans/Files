using Files.Shared.Enums;

namespace Files.Uwp.Helpers
{
    public interface IDateTimeFormatterFactory
    {
        IDateTimeFormatter GetDateTimeFormatter(TimeStyle timeStyle);
    }
}
