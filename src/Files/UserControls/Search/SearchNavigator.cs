using Files.Filesystem.Search;
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

        void ClearPage();
        void GoPage(ISearchSettings settings);
        void GoPage(ISearchFilter filter);
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

        public void ClearPage()
        {
            if (frame is not null)
            {
                frame.Content = null;
            }
        }
        public void GoPage(ISearchSettings settings)
        {
            var viewModel = new SettingsPageViewModel(settings);
            frame?.Navigate(typeof(SearchFilterPage), viewModel, emptyTransition);
        }
        public void GoPage(ISearchFilter filter)
        {
            ISearchPageViewModel viewModel = filter switch
            {
                IDateRangeFilter f => new DateRangePageViewModel(f),
                ISearchFilter f => new SearchPageViewModel(f),
                _  => null,
            };

            if (viewModel is not null)
            {
                frame?.Navigate(typeof(SearchFilterPage), viewModel, toRightTransition);
            }
        }
    }
}
