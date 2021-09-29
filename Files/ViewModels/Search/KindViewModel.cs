using Files.Filesystem.Search;

namespace Files.ViewModels.Search
{
    public interface IKindViewModel : IFilterViewModel
    {
    }

    public class KindPageViewModel : FilterViewModel, IKindViewModel
    {
        public KindPageViewModel(IFilter filter) : base(filter) {}
    }
}
