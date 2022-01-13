using Files.Filesystem.Search;
using System.Collections.Generic;
using System.Linq;

namespace Files.ViewModels.Search
{
    public class DateRangePageViewModel : SearchPageViewModel, IMultiSearchPageViewModel
    {
        public ISearchHeader SelectedHeader
        {
            get => Filter.Header;
            set
            {
                var key = value?.Key ?? string.Empty;
                if (key != string.Empty && key != SelectedHeader.Key)
                {
                    var filter = Filter as IDateRangeFilter;
                    filter.Origin = Headers.First(header => header.Key == key).Origin;
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

        public DateRangePageViewModel(ISearchPageViewModel parent, IDateRangeFilter filter)
            : base(parent, filter) {}
    }
}
