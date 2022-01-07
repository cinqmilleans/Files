using Files.Filesystem.Search;

namespace Files.ViewModels.Search
{
    public interface ISettingsFilterViewModel : ISearchFilterViewModel
    {
        ISearchSettings Settings { get; }
    }

    public class SettingsFilterViewModel : SearchFilterViewModel<ISearchFilterCollection>, ISettingsFilterViewModel
    {
        public ISearchSettings Settings { get; }

        public SettingsFilterViewModel(ISearchContext context, ISearchSettings settings)
            : base(context, settings.Filter) => Settings = settings;
    }
}
