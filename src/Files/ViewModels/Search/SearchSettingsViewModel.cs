using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface ISearchSettingsViewModel
    {
        ISearchContext Context { get; }
        ISearchSettings Settings { get; }
    }

    public class SearchSettingsViewModel : ObservableObject, ISearchSettingsViewModel
    {
        public ISearchContext Context { get; }
        public ISearchSettings Settings { get; }

        public SearchSettingsViewModel(ISearchContext context, ISearchSettings settings)
            => (Context, Settings) = (context, settings);
    }
}
