using Files.Filesystem.Search;
using System.Collections.Generic;
using System.Linq;

namespace Files.ViewModels.Search
{
    public interface ISearchPageViewModel
    {
        IEnumerable<ISearchHeader> Headers { get; }
        ISearchFilter Filter { get; }
    }

    public interface ISettingsPageViewModel : ISearchPageViewModel
    {
        ISearchSettings Settings { get; }
    }

    public interface ISearchPageViewModelFactory
    {
        ISearchPageViewModel GetPageViewModel(ISearchFilter filter);
    }

    public class SearchPageViewModelFactory : ISearchPageViewModelFactory
    {
        public ISearchPageViewModel GetPageViewModel(ISearchFilter filter) => filter switch
        {
            _ => new SearchPageViewModel(filter),
        };
    }

    public class SearchPageViewModel : ISearchPageViewModel
    {
        public IEnumerable<ISearchHeader> Headers { get; } = Enumerable.Empty<ISearchHeader>();

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
