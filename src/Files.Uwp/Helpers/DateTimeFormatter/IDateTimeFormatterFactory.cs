using Files.Shared.Enums;

namespace Files.Uwp.Helpers
{
    public interface IDateTimeFormatterFactory
    {
        INamedDateTimeFormatter GetDateTimeFormatter();
        INamedDateTimeFormatter GetDateTimeFormatter(TimeStyle timeStyle);
    }
}
