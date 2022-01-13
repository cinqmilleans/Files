using Files.Filesystem.Search;
using System.Collections.Generic;
using System.Linq;

namespace Files.ViewModels.Search
{
    public class GroupPageViewModel : SearchPageViewModel, IMultiSearchPageViewModel
    {
        public ISearchHeader SelectedHeader
        {
            get => Filter.Header;
            set
            {
                var key = value?.Key ?? string.Empty;
                if (key != string.Empty && key != SelectedHeader.Key)
                {
                    var filter = Filter as ISearchFilterCollection;
                    filter.Aggregate = Headers.First(header => header.Key == key).Aggregate;
                    OnPropertyChanged(nameof(SelectedHeader));
                }
            }
        }

        IEnumerable<ISearchHeader> IMultiSearchPageViewModel.Headers => Headers;
        public IList<IGroupHeader> Headers { get; } = new List<IGroupHeader>
        {
            new AndHeader(),
            new OrHeader(),
            new NotHeader(),
        };

        public GroupPageViewModel(ISearchPageViewModel parent, ISearchFilterCollection filter)
            : base(parent, filter) {}
    }
}
