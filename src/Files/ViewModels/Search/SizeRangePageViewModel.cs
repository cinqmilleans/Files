using Files.Extensions;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISearchPageViewModel
    {
        new IDateRangeFilter Filter { get; }
    }

    public class SizeRangePageViewModel : ObservableObject, ISizeRangePageViewModel
    {
        public ISearchNavigator Navigator { get; }

        ISearchFilter ISearchPageViewModel.Filter => Filter;
        public IDateRangeFilter Filter { get; }

        public IEnumerable<ISearchHeader> Alternatives => Filter.Header.CreateEnumerable();

        public SizeRangePageViewModel(ISearchNavigator navigator, IDateRangeFilter filter)
            => (Navigator, Filter) = (navigator, filter);
    }
}
