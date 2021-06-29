namespace Files.Filesystem.Search
{
    #region period
    /*public class BeforeSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "before";
        public virtual string Label => "Before";

        public string[] Suggestions { get; } = new string[0];

        public ISearchOptionValue GetEmptyValue() => new BeforeSearchOptionValue();

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntax syntax)
            {
                return $"System.ItemDate:{syntax.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }

    public class AfterSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "after";
        public virtual string Label => "After";

        public string[] Suggestions { get; } = new string[0];

        public ISearchOptionValue GetEmptyValue() => new AfterSearchOptionValue();

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntax syntax)
            {
                return $"System.ItemDate:{syntax.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }*/

    /*public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "date";
        public virtual string Label => "Date of creation";

        public string[] Suggestions { get; } = new string[] { "date:today", "date:yesterday", "date:weekago", "date:monthago", "date:yearago" };

        public ISearchOptionValue GetEmptyValue() => new PeriodSearchOptionValue();

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntax syntax)
            {
                return $"System.ItemDate:{syntax.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }

    public class ModifiedSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "modified";
        public virtual string Label => "Date of last modification";

        public ISearchOptionValue GetEmptyValue() => new PeriodSearchOptionValue();

        public string[] Suggestions { get; } = new string[0];

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IAdvancedQuerySyntax syntax)
            {
                return $"System.DateModified:{syntax.AdvancedQuerySyntax}";
            }
            return string.Empty;
        }
    }*/
    #endregion
}
