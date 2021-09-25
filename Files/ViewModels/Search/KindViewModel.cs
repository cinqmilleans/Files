using Files.Filesystem.Search;

namespace Files.ViewModels.Search
{
    public interface IKindViewModel : IFilterViewModel
    {
    }

    public class KindPageViewModel : FilterViewModel<IFilter>, IKindViewModel
    {
        public KindPageViewModel(INavigatorViewModel navigator, IFilter filter) : base(navigator, filter) {}
    }
}
