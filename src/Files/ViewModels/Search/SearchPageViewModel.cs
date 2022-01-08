using Files.Filesystem.Search;
using System.Collections.Generic;

namespace Files.ViewModels.Search
{
    public interface ISettingsPageViewModel : ISearchPageViewModel
    {
        ISearchSettings Settings { get; }
    }
    public interface ISearchPageViewModel
    {
        ISearchFilter Filter { get; }
    }
    public interface IMultiSearchPageViewModel : ISearchPageViewModel
    {
        IEnumerable<ISearchHeader> Headers { get; }
    }

    public class SearchPageViewModel : ISearchPageViewModel
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
