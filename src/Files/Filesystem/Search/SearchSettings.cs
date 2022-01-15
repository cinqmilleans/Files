using Files.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }

        ISearchFilterCollection Filter { get; }

        void Clear();
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        private readonly int pinnedCount;

        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }

        public ISearchFilterCollection Filter { get; }

        public SearchSettings()
        {
            var pinnedKeys = new SearchKeys[] { SearchKeys.Size, SearchKeys.DateModified };
            pinnedCount = pinnedKeys.Length;

            var provider = Ioc.Default.GetService<ISearchHeaderProvider>();
            var pinneds = pinnedKeys.Select(key => GetFilter(key)).ToList();
            Filter = new SearchFilterCollection(SearchKeys.GroupAnd, pinneds);

            ISearchFilter GetFilter(SearchKeys key) => provider.GetHeader(key).CreateFilter();
        }

        public void Clear()
        {
            SearchInSubFolders = true;

            Filter.Take(pinnedCount).ForEach(subFilter => subFilter.Clear());
            Filter.Skip(pinnedCount).ToList().ForEach(subFilter => Filter.Remove(subFilter));
        }
    }
}
