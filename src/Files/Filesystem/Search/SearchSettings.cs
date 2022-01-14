using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }

        IReadOnlyList<SearchKeys> PinnedKeys { get; }
        ISearchFilterCollection Filter { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }

        public IReadOnlyList<SearchKeys> PinnedKeys { get; }
            = new List<SearchKeys> { SearchKeys.Size, SearchKeys.DateModified }.AsReadOnly();

        public ISearchFilterCollection Filter { get; }

        public SearchSettings()
        {
            var provider = Ioc.Default.GetService<ISearchHeaderProvider>();
            var filters = PinnedKeys.Select(key => GetFilter(key)).ToList();
            Filter = new SearchFilterCollection(SearchKeys.GroupAnd, filters);

            ISearchFilter GetFilter(SearchKeys key) => provider.GetHeader(key).CreateFilter();
        }
    }
}
