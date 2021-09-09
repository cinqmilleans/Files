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
        private readonly ISearchNavigatorViewModel viewModel;

        public SearchNavigator(Frame frame, ISearchNavigatorViewModel viewModel)
        {
            this.frame = frame;
            this.viewModel = viewModel;

            viewModel.PageOpened += ViewModel_PageOpened;
            viewModel.BackRequested += ViewModel_BackRequested;
            viewModel.ForwardRequested += ViewModel_ForwardRequested;
        }

        ~SearchNavigator()
        {
            viewModel.BackRequested -= ViewModel_BackRequested;
            viewModel.ForwardRequested -= ViewModel_ForwardRequested;
        }

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
