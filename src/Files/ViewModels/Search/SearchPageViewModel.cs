using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface ISettingsPageViewModel : ISearchPageViewModel
    {
        ISearchSettings Settings { get; }
    }
    public interface ISearchPageViewModel : INotifyPropertyChanged
    {
        ISearchFilter Filter { get; }
    }
    public interface IMultiSearchPageViewModel : ISearchPageViewModel
    {
        ISearchHeader SelectedHeader { get; set; }
        IEnumerable<ISearchHeader> Headers { get; }
    }

    public class SearchPageViewModel : ObservableObject, ISearchPageViewModel
    {
        public ISearchFilter Filter { get; }

        public SearchPageViewModel(ISearchFilter filter) => Filter = filter;
    }

    public class SettingsPageViewModel : SearchPageViewModel, ISettingsPageViewModel
    {
        public ISearchSettings Settings { get; }

        public SettingsPageViewModel(ISearchSettings settings)
            : base(settings.Filter) => Settings = settings;
    }
}
