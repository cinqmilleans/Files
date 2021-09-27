using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISettingsViewModel
    {
        INavigatorViewModel Navigator { get; }

        ILocationViewModel Location { get; }
        IFilterViewModel Filter { get; }

        ICommand SearchCommand { get; }
    }

    public class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        public INavigatorViewModel Navigator { get; }

        public ILocationViewModel Location { get; }
        public IFilterViewModel Filter { get; }

        public ICommand SearchCommand { get; }

        public SettingsViewModel(ISettings settings, INavigatorViewModel navigator)
        {
            var factory = new FilterViewModelFactory();

            Navigator = navigator;
            Location = new LocationViewModel(settings.Location);
            Filter = factory.GetViewModel(settings.Filter);
            SearchCommand = navigator.SearchCommand;
        }
    }
}
