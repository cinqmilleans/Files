namespace Files.Filesystem.Search
{
    public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "date";
        public virtual string Label => "Date of creation";

        public IFactory<ISearchOptionValue> Format { get; } = PeriodSearchOptionValueFactory.Default;

        public string[] Suggestions { get; } = new string[] { "date:today", "date:yesterday", "date:weekago", "date:monthago", "date:yearago" };

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IPeriod period)
            {
                return $"System.ItemDate:>={period.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }

    public class ModifiedSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "modified";
        public virtual string Label => "Date of last modification";

        public IFactory<ISearchOptionValue> Format { get; } = PeriodSearchOptionValueFactory.Default;

        public string[] Suggestions { get; } = new string[0];

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IPeriod period)
            {
                return $"System.DateModified:{period.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }
}
