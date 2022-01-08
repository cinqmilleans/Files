using Files.Filesystem.Search;
using Files.UserControls.Search;
using Files.ViewModels;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class SearchBox : UserControl
    {
        private readonly SearchNavigator navigator = new();
        private readonly ISettingsPageViewModel settingsPageViewModel;

        public static readonly DependencyProperty SearchBoxViewModelProperty =
            DependencyProperty.Register(nameof(SearchBoxViewModel), typeof(SearchBoxViewModel), typeof(SearchBox), new PropertyMetadata(null));

        public SearchBoxViewModel SearchBoxViewModel
        {
            get => (SearchBoxViewModel)GetValue(SearchBoxViewModelProperty);
            set => SetValue(SearchBoxViewModelProperty, value);
        }

        public SearchBox()
        {
            InitializeComponent();

            ISearchSettings settings = Ioc.Default.GetService<ISearchSettings>();
            settingsPageViewModel = new SettingsPageViewModel(settings);
        }

        private void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
            => SearchBoxViewModel.SearchRegion_TextChanged(sender, e);
        private void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
            => SearchBoxViewModel.SearchRegion_QuerySubmitted(sender, e);
        private void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
            => SearchBoxViewModel.SearchRegion_Escaped(sender, e);

        private void MenuFrame_Loaded(object sender, RoutedEventArgs e)
        {
            navigator.Initialize(SearchBoxViewModel, sender as Frame);
            navigator.GoPage(settingsPageViewModel);
        }
        private void MenuButton_Loaded(object sender, RoutedEventArgs e)
        {
            bool allowFocusOnInteractionAvailable =
                ApiInformation.IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction");
            if (allowFocusOnInteractionAvailable && sender is FrameworkElement element)
            {
                element.AllowFocusOnInteraction = true;
            }
        }

        private void MenuBadge_Loaded(object sender, RoutedEventArgs e) =>
            (sender as FrameworkElement).DataContext = settingsPageViewModel;

        private void MenuFlyout_Opened(object sender, object e) => navigator.GoPage(settingsPageViewModel);
        private void MenuFlyout_Closed(object sender, object e) => navigator.GoPage(null);
    }
}
