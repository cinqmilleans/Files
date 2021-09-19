using Files.ViewModels.Search;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Files.UserControls.Search
{
    public class SearchNavigator
    {
        private readonly NavigationTransitionInfo emptyTransition
            = new SuppressNavigationTransitionInfo();
        private readonly NavigationTransitionInfo toRightTransition
            = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        private readonly Frame frame;

        public ISearchNavigatorViewModel ViewModel { get; }

        public SearchNavigator(Frame frame, ISearchNavigatorViewModel viewModel)
        {
            this.frame = frame;
            ViewModel = viewModel;

            ViewModel.PageOpened += ViewModel_PageOpened;
            ViewModel.BackRequested += ViewModel_BackRequested;
            ViewModel.ForwardRequested += ViewModel_ForwardRequested;
        }

        ~SearchNavigator()
        {
            ViewModel.BackRequested -= ViewModel_BackRequested;
            ViewModel.ForwardRequested -= ViewModel_ForwardRequested;
        }

        public void GoRoot() => ViewModel.OpenPage(new RootSearchPageViewModel(ViewModel));
        public void Clean() => ViewModel.OpenPage(null);

        private void ViewModel_PageOpened(ISearchNavigatorViewModel sender, PageOpenedSearchNavigatorEventArgs e)
            => Go(e.PageViewModel);
        private void ViewModel_BackRequested(ISearchNavigatorViewModel sender, EventArgs e)
            => frame.GoBack(toRightTransition);
        private void ViewModel_ForwardRequested(ISearchNavigatorViewModel sender, EventArgs e)
            => frame.GoForward();

        private void Go(object viewModel)
        {
            switch (viewModel)
            {
                case IRootSearchPageViewModel _ :
                    frame.Navigate(typeof(RootSearchPage), viewModel, emptyTransition);
                    break;
                case ISettingSearchPageViewModel _:
                    frame.Navigate(typeof(SettingSearchPage), viewModel, toRightTransition);
                    break;
            }
        }
    }
}
