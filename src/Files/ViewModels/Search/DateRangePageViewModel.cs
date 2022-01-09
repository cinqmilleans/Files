using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace Files.ViewModels.Search
{
    public class DateRangePageViewModel : ObservableObject, IMultiSearchPageViewModel
    {
        public ISearchHeader SelectedHeader
        {
            get => Filter.Header;
            set
            {
                var key = value?.Key ?? string.Empty;
                if (key != SelectedHeader.Key)
                {
                    Filter.Origin = Headers.First(header => header.Key == key).Origin;
                    OnPropertyChanged(nameof(SelectedHeader));
                }
            }
        }

        IEnumerable<ISearchHeader> IMultiSearchPageViewModel.Headers => Headers;
        public IList<IDateRangeHeader> Headers { get; } = new List<IDateRangeHeader>
        {
            new CreatedHeader(),
            new ModifiedHeader(),
            new AccessedHeader(),
        };

        ISearchFilter ISearchPageViewModel.Filter => Filter;
        private IDateRangeFilter Filter { get; }

        public DateRangePageViewModel(IDateRangeFilter filter) => Filter = filter;
    }
}
