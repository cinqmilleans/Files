using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface IRootSearchPageViewModel
    {
        ISettingSearchPageViewModel SizePageViewModel { get; }
    }

    public class RootSearchPageViewModel : ObservableObject, IRootSearchPageViewModel
    {
        public ISettingSearchPageViewModel SizePageViewModel { get; }

        public RootSearchPageViewModel(ISearchNavigatorViewModel navigator)
        {
            SizePageViewModel = new SizeRangePageViewModel(navigator);
        }
    }
}
