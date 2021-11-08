namespace Files.ViewModels.Search
{
    public interface ISyntaxViewModel
    {
        string Name { get; }
    }

    public interface IDateRangeSyntaxViewModel
    {
    }

    public class DateRangeSyntaxViewModel : IDateRangeSyntaxViewModel
    {
        public string Name { get; } = "date";
    }
}
