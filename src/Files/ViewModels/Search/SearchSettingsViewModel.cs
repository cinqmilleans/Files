using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Specialized;

namespace Files.ViewModels.Search
{
    public interface ISearchSettingsViewModel
    {
        int BadgeCount { get; }

        ISearchPageNavigator Navigator { get; }

        IPickerViewModel LocationViewModel { get; }
        IPickerViewModel FilterViewModel { get; }
    }

    public class SearchSettingsViewModel : ObservableObject, ISearchSettingsViewModel
    {
        private readonly ISearchFilterCollection filter;

        public int BadgeCount => filter.Count;

        public ISearchPageNavigator Navigator { get; }

        public IPickerViewModel LocationViewModel { get; }
        public IPickerViewModel FilterViewModel { get; }

        public SearchSettingsViewModel(ISearchPageContext context, ISearchSettings settings)
        {
            Navigator = context;
            filter = settings.Filter;
            filter.CollectionChanged += Filter_CollectionChanged;

            LocationViewModel = new LocationPickerViewModel(settings.Location);
            FilterViewModel = new GroupPageViewModel(context, filter).Picker;
        }

        private void Filter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            OnPropertyChanged(nameof(BadgeCount));
    }
}
