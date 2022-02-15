using Files.Filesystem.Search;
using Files.UserControls.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchTagViewModel
    {
        ISearchFilterViewModel Filter { get; }

        string Title { get; }
        string Parameter { get; }

        ICommand OpenCommand { get; }
        ICommand DeleteCommand { get; }
    }

    public class SearchTagViewModel : ISearchTagViewModel
    {
        private readonly ISearchTag tag;

        public ISearchFilterViewModel Filter { get; }

        public string Title => tag.Title;
        public string Parameter => tag.Parameter;

        public ICommand OpenCommand { get; }
        public ICommand DeleteCommand { get; }

        public SearchTagViewModel(ISearchFilterViewModel filter, ISearchTag tag)
        {
            Filter = filter;
            this.tag = tag;

            OpenCommand = new RelayCommand<ISearchFilterViewModel>(Open);
            DeleteCommand = new RelayCommand(tag.Delete);
        }

        private void Open(ISearchFilterViewModel middleFilter = null)
        {
            var navigator = Ioc.Default.GetService<ISearchNavigator>();

            if (middleFilter is not null)
            {
                navigator.GoPage(middleFilter);
            }

            navigator.GoPage(Filter);
        }
    }
}
