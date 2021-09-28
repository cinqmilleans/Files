using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISettingsViewModel
    {
        INavigatorViewModel Navigator { get; }

        ILocationViewModel Location { get; }
        IFilterPageViewModel FilterPage { get; }

        ICommand SearchCommand { get; }
    }

    public class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        public INavigatorViewModel Navigator { get; }

        public ILocationViewModel Location { get; }
        public IFilterPageViewModel FilterPage { get; }

        public ICommand SearchCommand { get; }

        public SettingsViewModel(ISettings settings, INavigatorViewModel navigator)
        {
            var factory = new FilterViewModelFactory();

            Navigator = navigator;
            Location = new LocationViewModel(settings.Location);
            FilterPage = new FilterPageViewModel
            {
                Navigator = navigator,
                Filter = factory.GetViewModel(settings.Filter),
            };
            SearchCommand = navigator.SearchCommand;
        }
    }
}
