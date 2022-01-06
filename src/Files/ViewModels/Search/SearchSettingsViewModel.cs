using Files.Filesystem.Search;

namespace Files.ViewModels.Search
{
    public interface ISearchSettingsViewModel : ISearchFilterViewModel
    {
        ISearchSettings Settings { get; }
    }

    public class SearchSettingsViewModel : SearchFilterViewModel<ISearchFilterCollection>, ISearchSettingsViewModel
    {
        public ISearchSettings Settings { get; }

        public SearchSettingsViewModel(ISearchContext context, ISearchSettings settings)
            : base(context, settings.Filter) => Settings = settings;
    }
}
