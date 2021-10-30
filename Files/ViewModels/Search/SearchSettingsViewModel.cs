using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISettingsViewModel
    {
        IPickerViewModel LocationViewModel { get; }
        IPickerViewModel FilterViewModel { get; }
    }

    public class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        public IPickerViewModel LocationViewModel { get; }
        public IPickerViewModel FilterViewModel { get; }

        public SettingsViewModel(ISearchPageContext context, ISearchSettings settings)
        {
            var filter = settings.Filter as ISearchFilterCollection;

            LocationViewModel = new LocationPickerViewModel(settings.Location);
            FilterViewModel = new GroupPickerViewModel(context, filter)
            {
                Description = filter.Description
            };
        }
    }
}
