using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.Foundation;

namespace Files.ViewModels.Search
{
    public interface INavigatorViewModel : INotifyPropertyChanged
    {
        event TypedEventHandler<INavigatorViewModel, PageOpenedNavigatorEventArgs> PageOpened;
        event TypedEventHandler<INavigatorViewModel, EventArgs> SearchRequested;
        event TypedEventHandler<INavigatorViewModel, EventArgs> BackRequested;
        event TypedEventHandler<INavigatorViewModel, EventArgs> ForwardRequested;

        ISettings Settings { get; }

        ICommand SearchCommand { get; }
        ICommand BackCommand { get; }
        ICommand ForwardCommand { get; }

        void OpenPage(object pageViewModel);
        void Back();
        void Forward();
    }

    public class PageOpenedNavigatorEventArgs : EventArgs
    {
        public object PageViewModel { get; }

        public PageOpenedNavigatorEventArgs(object pageViewModel) => PageViewModel = pageViewModel;
    }

    public interface IPageOpenedNavigatorEventArgs
    {
        object PageViewModel { get; }
    }

    public class NavigatorViewModel : ObservableObject, INavigatorViewModel
    {
        public event TypedEventHandler<INavigatorViewModel, PageOpenedNavigatorEventArgs> PageOpened;
        public event TypedEventHandler<INavigatorViewModel, EventArgs> SearchRequested;
        public event TypedEventHandler<INavigatorViewModel, EventArgs> BackRequested;
        public event TypedEventHandler<INavigatorViewModel, EventArgs> ForwardRequested;

        public ISettings Settings { get; }

        public ICommand SearchCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ForwardCommand { get; }

        public NavigatorViewModel(ISettings settings)
        {
            Settings = settings;

            SearchCommand = new RelayCommand(Search);
            BackCommand = new RelayCommand(Back);
            ForwardCommand = new RelayCommand(Forward);
        }

        public void OpenPage(object pageViewModel)
            => PageOpened?.Invoke(this, new PageOpenedNavigatorEventArgs(pageViewModel));

        public void Search() => SearchRequested?.Invoke(this, EventArgs.Empty);
        public void Back() => BackRequested?.Invoke(this, EventArgs.Empty);
        public void Forward() => ForwardRequested?.Invoke(this, EventArgs.Empty);
    }
}
