namespace Files.Filesystem.Search
{
    public interface ISearchOptionKey
    {
        string Text { get; }
        string Label { get; }

        IFactory<ISearchOptionValue> ValueFactory { get; }

        string[] Suggestions { get; }
        string GetAdvancedQuerySyntax(ISearchOptionValue value);
    }

    public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "date";
        public virtual string Label => "Date of creation";

        public IFactory<ISearchOptionValue> ValueFactory { get; } = IntervalPeriodSearchOptionValueFactory.Default;

        public string[] Suggestions { get; } = new string[] { "modified:", "date:2019" };

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntaxValue aqsValue)
            {
                return $"System.ItemDate:{aqsValue.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }

    public class ModifiedSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "modified";
        public virtual string Label => "Date of last modification";

        public IFactory<ISearchOptionValue> ValueFactory { get; } = IntervalPeriodSearchOptionValueFactory.Default;

        public string[] Suggestions { get; } = new string[0];

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntaxValue aqsValue)
            {
                return $"System.DateModified:{aqsValue.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }
}
