using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISettingsViewModel
    {
        ILocationViewModel Location { get; }
        IFilterViewModel Filter { get; }

        ICommand SearchCommand { get; }
    }

    public class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        public ILocationViewModel Location { get; }
        public IFilterViewModel Filter { get; }

        public ICommand SearchCommand { get; }

        public SettingsViewModel(INavigatorViewModel navigator)
        {
            var factory = new FilterViewModelFactory(navigator);

            Location = new LocationViewModel(navigator.Settings.Location);
            Filter = factory.GetViewModel(navigator.Settings.Filter);
            SearchCommand = navigator.SearchCommand;
        }
    }
}
