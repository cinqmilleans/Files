using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        public IReadOnlyDictionary<string, ISearchHeader> Headers { get; }

        bool SearchInSubFolders { get; set; }

        IReadOnlyList<string> PinnedKeys { get; }
        ISearchFilterCollection Filter { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public IReadOnlyDictionary<string, ISearchHeader> Headers { get; }
            = new ReadOnlyDictionary<string, ISearchHeader>(new List<ISearchHeader>
            {
                new AndHeader(),
                new OrHeader(),
                new NotHeader(),
                new SizeHeader(),
                new CreatedHeader(),
                new ModifiedHeader(),
                new AccessedHeader(),
            }.ToDictionary(header => header.Key));

        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }

        public IReadOnlyList<string> PinnedKeys { get; } = new List<string>{ "size", "modified" }.AsReadOnly();
        public ISearchFilterCollection Filter { get; } = new SearchFilterCollection(GroupAggregates.And);

        public SearchSettings()
        {
            var filters = PinnedKeys.Select(key => Headers[key].GetFilter()).ToList();
            Filter = new SearchFilterCollection(GroupAggregates.And, filters);
        }
    }
}
