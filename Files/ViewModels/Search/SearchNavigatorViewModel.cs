using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.Foundation;

namespace Files.ViewModels.Search
{
    public interface ISearchNavigatorViewModel : INotifyPropertyChanged
    {
        event TypedEventHandler<ISearchNavigatorViewModel, PageOpenedSearchNavigatorEventArgs> PageOpened;
        event TypedEventHandler<ISearchNavigatorViewModel, EventArgs> BackRequested;
        event TypedEventHandler<ISearchNavigatorViewModel, EventArgs> ForwardRequested;

        ISearchSettings Settings { get; }

        ICommand BackCommand { get; }
        ICommand ForwardCommand { get; }

        void OpenPage(object pageViewModel);
        void Back();
        void Forward();
    }

    public class PageOpenedSearchNavigatorEventArgs : EventArgs
    {
        public object PageViewModel { get; }

        public PageOpenedSearchNavigatorEventArgs(object pageViewModel) => PageViewModel = pageViewModel;
    }

    public interface IPageOpenedSearchNavigatorEventArgs
    {
        object PageViewModel { get; }
    }

    public class SearchNavigatorViewModel : ObservableObject, ISearchNavigatorViewModel
    {
        public event TypedEventHandler<ISearchNavigatorViewModel, PageOpenedSearchNavigatorEventArgs> PageOpened;
        public event TypedEventHandler<ISearchNavigatorViewModel, EventArgs> BackRequested;
        public event TypedEventHandler<ISearchNavigatorViewModel, EventArgs> ForwardRequested;

        public ISearchSettings Settings { get; }

        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }

        public SearchNavigatorViewModel(ISearchSettings settings)
        {
            Settings = settings;

            BackCommand = new RelayCommand(Back);
            ForwardCommand = new RelayCommand(Forward);
        }

        public void OpenPage(object pageViewModel)
            => PageOpened?.Invoke(this, new PageOpenedSearchNavigatorEventArgs(pageViewModel));
        public void Back() => BackRequested?.Invoke(this, EventArgs.Empty);
        public void Forward() => ForwardRequested?.Invoke(this, EventArgs.Empty);
    }
}
