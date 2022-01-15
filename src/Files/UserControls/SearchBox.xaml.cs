using Files.Filesystem.Search;
using Files.UserControls.Search;
using Files.ViewModels;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class SearchBox : UserControl
    {
        private Button MenuButton;

        private readonly ISearchNavigator navigator = Ioc.Default.GetService<ISearchNavigator>();
        private readonly ISearchSettings settings = Ioc.Default.GetService<ISearchSettings>();
        private readonly IBadgeViewModel badgeViewModel;

        public static readonly DependencyProperty SearchBoxViewModelProperty =
            DependencyProperty.Register(nameof(SearchBoxViewModel), typeof(SearchBoxViewModel), typeof(SearchBox), new PropertyMetadata(null));

        public SearchBoxViewModel SearchBoxViewModel
        {
            get => (SearchBoxViewModel)GetValue(SearchBoxViewModelProperty);
            set
            {
                if (SearchBoxViewModel is not null)
                {
                    SearchBoxViewModel.PropertyChanged -= SearchBoxViewModel_PropertyChanged;
                }
                SetValue(SearchBoxViewModelProperty, value);
                if (SearchBoxViewModel is not null)
                {
                    SearchBoxViewModel.PropertyChanged += SearchBoxViewModel_PropertyChanged;
                }
            }
        }

        public SearchBox()
        {
            InitializeComponent();
            badgeViewModel = new BadgeViewModel(settings.Filter);
        }

        private void SearchBoxViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchBox.IsMenuOpen))
            {
                if (SearchBoxViewModel.IsMenuOpen)
                {
                    MenuButton?.Flyout?.Hide();
                }
                else
                {
                    MenuButton?.Flyout?.ShowAt(this);
                }
            }
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
            navigator.GoPage(settings);
        }
        private void MenuButton_Loaded(object sender, RoutedEventArgs e)
        {
            MenuButton = sender as Button;

            bool allowFocusOnInteractionAvailable =
                ApiInformation.IsPropertyPresent("Windows.UI.Xaml.FrameworkElement", "AllowFocusOnInteraction");
            if (allowFocusOnInteractionAvailable && sender is FrameworkElement element)
            {
                element.AllowFocusOnInteraction = true;
            }
        }

        private void MenuBadge_Loaded(object sender, RoutedEventArgs e)
            => (sender as FrameworkElement).DataContext = badgeViewModel;

        private void MenuFlyout_Opened(object sender, object e) => navigator.GoPage(settings);
        private void MenuFlyout_Closed(object sender, object e) => navigator.ClearPage();
    }
}
