using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }

        SearchFilterCollection Filter { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }

        public SearchFilterCollection Filter { get; } = new SearchFilterCollection(GroupAggregates.And);
    }
}
