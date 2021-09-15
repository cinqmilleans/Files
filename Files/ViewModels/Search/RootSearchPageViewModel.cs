using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    public interface IRootSearchPageViewModel
    {
        IEnumerable<ISettingSearchPageViewModel> SettingPageViewModels { get; }

        ISettingSearchPageViewModel KindPageViewModel { get; }
        ISettingSearchPageViewModel CreatedPageViewModel { get; }
        ISettingSearchPageViewModel ModifiedPageViewModel { get; }
        ISettingSearchPageViewModel SizePageViewModel { get; }
    }

    public class RootSearchPageViewModel : ObservableObject, IRootSearchPageViewModel
    {
        public IEnumerable<ISettingSearchPageViewModel> SettingPageViewModels { get; }

        public ISettingSearchPageViewModel KindPageViewModel { get; }
        public ISettingSearchPageViewModel CreatedPageViewModel { get; }
        public ISettingSearchPageViewModel ModifiedPageViewModel { get; }
        public ISettingSearchPageViewModel SizePageViewModel { get; }

        public RootSearchPageViewModel(ISearchNavigatorViewModel navigator)
        {
            KindPageViewModel = new KindPageViewModel(navigator);
            CreatedPageViewModel = new CreatedPageViewModel(navigator);
            ModifiedPageViewModel = new ModifiedPageViewModel(navigator);
            SizePageViewModel = new SizeRangePageViewModel(navigator);

            SettingPageViewModels = new List<ISettingSearchPageViewModel>
            {
                KindPageViewModel,
                CreatedPageViewModel,
                ModifiedPageViewModel,
                SizePageViewModel,
            };
        }
    }
}
