using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface ISearchSettingsViewModel
    {
        IPickerViewModel LocationViewModel { get; }
        IPickerViewModel FilterViewModel { get; }
    }

    public class SearchSettingsViewModel : ObservableObject, ISearchSettingsViewModel
    {
        public IPickerViewModel LocationViewModel { get; }
        public IPickerViewModel FilterViewModel { get; }

        public SearchSettingsViewModel(ISearchPageContext context, ISearchSettings settings)
        {
            var filter = settings.Filter as ISearchFilterCollection;

            LocationViewModel = new LocationPickerViewModel(settings.Location);
            FilterViewModel = new GroupPickerViewModel(context, filter);
        }
    }
}
