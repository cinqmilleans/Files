using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Files.UserControls.Search
{
    public interface ISearchNavigator
    {
        void GoBack();
        void GoPage(object viewModel);
    }

    public class SearchNavigator : ISearchNavigator
    {
        private readonly NavigationTransitionInfo emptyTransition =
            new SuppressNavigationTransitionInfo();
        private readonly NavigationTransitionInfo toRightTransition =
            new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        public Frame Frame { get; set; }

        public void GoBack()
        {
            if (Frame is not null && Frame.CanGoBack)
            {
                Frame.GoBack(toRightTransition);
            }
        }

        public void GoPage(object viewModel)
        {
            if (Frame is null)
            {
                return;
            }
            /*switch (viewModel)
            {
                case ISettingsViewModel:
                    Frame.Navigate(typeof(SettingsPage), viewModel, emptyTransition);
                    break;
                case IMultiSearchPageViewModel:
                    Frame.Navigate(typeof(MultiFilterPage), viewModel, toRightTransition);
                    break;
                case ISearchPageViewModel:
                    Frame.Navigate(typeof(FilterPage), viewModel, toRightTransition);
                    break;
                default:
                    Frame.Content = null;
                    break;
            }*/
        }
    }
}
