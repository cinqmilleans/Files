using Files.Filesystem.Search;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    [SearchPageViewModel(SearchKeys.GroupAnd)]
    [SearchPageViewModel(SearchKeys.GroupOr)]
    [SearchPageViewModel(SearchKeys.GroupNot)]
    public class GroupPageViewModel : MultiSearchPageViewModel
    {
        public GroupPageViewModel(ISearchPageViewModel parent, ISearchFilterCollection filter)
            : base(parent, filter) {}

        protected override IEnumerable<SearchKeys> GetKeys() => new List<SearchKeys>
        {
            SearchKeys.GroupAnd,
            SearchKeys.GroupOr,
            SearchKeys.GroupNot,
        };
    }
}
