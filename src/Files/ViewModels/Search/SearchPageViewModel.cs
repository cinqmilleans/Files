using Files.Filesystem.Search;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchPageViewModel : INotifyPropertyChanged
    {
        ISearchNavigator Navigator { get; }
        ISearchFilter Filter { get; }
        IEnumerable<ISearchHeader> Alternatives { get; }
    }

    public interface ISearchNavigator
    {
        ICommand SearchCommand { get; }
        ICommand BackCommand { get; }

        void Search();
        void Back();
    }
}
