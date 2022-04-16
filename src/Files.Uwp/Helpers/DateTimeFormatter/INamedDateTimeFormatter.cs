namespace Files.Uwp.Helpers
{
    public interface INamedDateTimeFormatter : IDateTimeFormatter
    {
        string Name { get; }
    }
}
