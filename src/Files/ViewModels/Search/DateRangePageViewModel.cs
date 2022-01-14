using Files.Filesystem.Search;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    public class DateRangePageViewModel : MultiSearchPageViewModel
    {
        public DateRangePageViewModel(ISearchPageViewModel parent, IDateRangeFilter filter)
            : base(parent, filter) {}

        protected override IEnumerable<SearchKeys> GetKeys() => new List<SearchKeys>
        {
            SearchKeys.DateCreated,
            SearchKeys.DateModified,
            SearchKeys.DateAccessed,
        };
    }
}
