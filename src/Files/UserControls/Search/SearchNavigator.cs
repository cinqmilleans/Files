﻿using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Files.UserControls.Search
{
    public interface ISearchNavigator : INotifyPropertyChanged
    {
        ISearchPageViewModel PageViewModel { get; }

        ICommand SearchCommand { get; }
        ICommand BackCommand { get; }

        void Initialize(Frame frame);
        void Initialize(ISearchBox box);

        void Search();
        void Back();

        void ClearPage();
        void GoPage(ISearchSettings settings);
        void GoPage(ISearchFilter filter);
    }

    public class SearchNavigator : ObservableObject, ISearchNavigator
    {
        private readonly ISearchFilterViewModelFactory filterFactory =
            Ioc.Default.GetService<ISearchFilterViewModelFactory>();

        private readonly ISearchPageViewModelFactory viewModelFactory =
            Ioc.Default.GetService<ISearchPageViewModelFactory>();

        private readonly NavigationTransitionInfo emptyTransition =
            new SuppressNavigationTransitionInfo();
        private readonly NavigationTransitionInfo toRightTransition =
            new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        private ISearchPageViewModel pageViewModel;
        public ISearchPageViewModel PageViewModel => pageViewModel;

        public ICommand SearchCommand { get; }

        private RelayCommand backCommand;
        public ICommand BackCommand => backCommand;

        private ISearchBox box;
        private Frame frame;

        public SearchNavigator()
        {
            SearchCommand = new RelayCommand(Search);
            backCommand = new RelayCommand(Back, CanBack);
        }

        public void Initialize(Frame frame) => this.frame = frame;
        public void Initialize(ISearchBox box) => this.box = box;

        public void Search()
        {
            if (box is not null)
            {
                if (string.IsNullOrWhiteSpace(box.Query))
                {
                    box.Query = "*";
                }
                box.Search();
            }
        }
        public void Back()
        {
            if (CanBack())
            {
                frame.GoBack(toRightTransition);
            }
        }
        private bool CanBack() => frame is not null && frame.CanGoBack;

        public void ClearPage() => GoPage(null, emptyTransition);

        public void GoPage(ISearchSettings settings)
        {
            var viewModel = new SearchSettingsPageViewModel(new SearchSettingsViewModel(settings));
            GoPage(viewModel, emptyTransition);
        }
        public void GoPage(ISearchFilter filter)
        {
            var filterViewModel = filterFactory.GetFilterViewModel(filter);
            var parentViewModel = (frame?.Content as SearchFilterPage)?.ViewModel;
            var childViewModel = viewModelFactory.GetPageViewModel(parentViewModel, filterViewModel);
            GoPage(childViewModel, toRightTransition);
        }
        private void GoPage(ISearchPageViewModel viewModel, NavigationTransitionInfo transition)
        {
            if (viewModel == pageViewModel)
            {
                return;
            }

            if (frame is not null && viewModel is not null)
            {
                pageViewModel = viewModel;
                frame.Navigate(typeof(SearchFilterPage), pageViewModel, toRightTransition);
                OnPropertyChanged(nameof(PageViewModel));
                backCommand.NotifyCanExecuteChanged();
            }
            else if (viewModel is not null)
            {
                pageViewModel = null;
                OnPropertyChanged(nameof(PageViewModel));
            }
        }
    }
}
