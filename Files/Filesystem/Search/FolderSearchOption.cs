using System.Collections.Generic;
using Windows.Storage.Search;

namespace Files.Filesystem.Search
{
    public interface IFolderSearchParameter
    {
        string Value { get; }
        void ApplyParameter(QueryOptions option);
    }
    public interface INamedFolderSearchParameter : IFolderSearchParameter
    {
        string Name { get; }
    }

    public class FolderSearchOption
    {
        public List<IFolderSearchParameter> Parameters = new List<IFolderSearchParameter>();

        public QueryOptions ToQueryOptions()
        {
            var option = new QueryOptions { UserSearchFilter = string.Empty };

            foreach (var parameter in Parameters)
                parameter.ApplyParameter(option);

            option.UserSearchFilter = option.UserSearchFilter.Trim();

            return option;
        }
    }

    public class QueryFolderSearchParameter : IFolderSearchParameter
    {
        public string Value { get; set; } = string.Empty;

        public void ApplyParameter(QueryOptions option)
        {
            option.UserSearchFilter += $" {Value}";
        }
    }

    public abstract class FilterFolderSearchParameter : INamedFolderSearchParameter
    {
        public string Name { get; }
        public string Value { get; set; }

        public FilterFolderSearchParameter(string name)
        {
            Name = name;
        }

        public void ApplyParameter(QueryOptions option)
        {
            option.UserSearchFilter += $" {Name}:{Value}";
        }
    }

    public class PeriodFolderSearchParameter : FilterFolderSearchParameter
    {
        public PeriodFolderSearchParameter (string name) : base(name)
        {
        }
    }

    public enum Period
    {
        None,
        DayAgo,
        WeekAgo,
        MonthAgo,
        YearAgo,
    }
}
