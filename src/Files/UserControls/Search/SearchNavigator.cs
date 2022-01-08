using Files.ViewModels.Search;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Files.UserControls.Search
{
    public interface ISearchNavigator
    {
        void Initialize(ISearchBox box, Frame frame);

        void Search();
        void Back();
        void GoPage(object ISearchFilter);
    }

    public class SearchNavigator : ISearchNavigator
    {
        private readonly NavigationTransitionInfo emptyTransition =
            new SuppressNavigationTransitionInfo();
        private readonly NavigationTransitionInfo toRightTransition =
            new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        private ISearchBox box;
        private Frame frame;

        public void Initialize(ISearchBox box, Frame frame) => (this.box, this.frame) = (box, frame);

        public void Search()
        {
            if (box is not null)
            {
                if (string.IsNullOrWhiteSpace(box.Query))
                {
                    box.Query = "*";
                }
                //SearchBox.Search();
                //SearchBox.IsMenuOpen = false;
            }
        }

        public void Back()
        {
            if (frame is not null && frame.CanGoBack)
            {
                frame.GoBack(toRightTransition);
            }
        }

        public void GoPage(object viewModel)
        {
            if (frame is null)
            {
                return;
            }
            switch (viewModel)
            {
                case ISettingsPageViewModel:
                    frame.Navigate(typeof(SearchFilterPage), viewModel, emptyTransition);
                    break;
                case ISearchPageViewModel:
                    frame.Navigate(typeof(SearchFilterPage), viewModel, toRightTransition);
                    break;
                default:
                    frame.Content = null;
                    break;
            }
        }
    }
}
