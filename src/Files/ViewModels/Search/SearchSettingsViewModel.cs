using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface ISearchSettingsViewModel
    {
        ISearchNavigator Navigator { get; }
        ISearchSettings Settings { get; }
    }

    public class SearchSettingsViewModel : ObservableObject, ISearchSettingsViewModel
    {
        public ISearchNavigator Navigator { get; }
        public ISearchSettings Settings { get; }

        public SearchSettingsViewModel(ISearchNavigator navigator, ISearchSettings settings)
            => (Navigator, Settings) = (navigator, settings);
    }
}
