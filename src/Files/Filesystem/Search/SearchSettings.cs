using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings
    {
        ObservableCollection<string> PinnedKeys { get; }

        ISearchLocation Location { get; }
        ISearchFilterCollection Filter { get; }
    }

    public interface ISearchLocation : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public ObservableCollection<string> PinnedKeys { get; }

        public ISearchLocation Location { get; } = new SearchLocation();
        public ISearchFilterCollection Filter { get; }

        public SearchSettings()
        {
            var pinnedKeys = new string[] { "size", "modified" };

            var manager = Ioc.Default.GetService<ISearchFilterManager>();

            PinnedKeys = new ObservableCollection<string>(pinnedKeys);
            Filter = new AndFilterCollection(pinnedKeys.Select(key => manager.GetFilter(key)));
        }
    }

    public class SearchLocation : ObservableObject, ISearchLocation
    {
        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }
    }
}
