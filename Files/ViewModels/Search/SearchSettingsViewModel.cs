using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Specialized;
using System.Windows.Input;

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

        public int BadgeCount => Math.Min(9, filter.Count);

        public ISearchPageNavigator Navigator { get; }

        public IPickerViewModel LocationViewModel { get; }
        public IPickerViewModel FilterViewModel { get; }

        public ICommand SearchCommand { get; }

        public SearchSettingsViewModel(ISearchPageContext context, ISearchSettings settings)
        {
            Navigator = context;
            SearchCommand = new RelayCommand(context.Search);

            filter = settings.Filter;
            filter.CollectionChanged += Filter_CollectionChanged;

            LocationViewModel = new LocationPickerViewModel(settings.Location);
            FilterViewModel = new GroupPickerViewModel(context, filter);
        }

        private void Filter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => OnPropertyChanged(nameof(BadgeCount));
    }
}
