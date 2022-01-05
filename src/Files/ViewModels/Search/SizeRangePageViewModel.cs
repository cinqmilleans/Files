using Files.Extensions;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISearchPageViewModel
    {
        new ISizeRangeFilter Filter { get; }
    }

    public class SizeRangePageViewModel : ObservableObject, ISizeRangePageViewModel
    {
        public ISearchContext Context { get; }

        ISearchFilter ISearchPageViewModel.Filter => Filter;
        public ISizeRangeFilter Filter { get; }

        public IEnumerable<ISearchHeader> Alternatives => Filter.Header.CreateEnumerable();

        public SizeRangePageViewModel(ISearchContext context, ISizeRangeFilter filter)
            => (Context, Filter) = (context, filter);
    }
}
