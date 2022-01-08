using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPage : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchPageViewModel), typeof(SearchFilterPage), new PropertyMetadata(null));

        private ISearchPageViewModel ViewModel
        {
            get => (ISearchPageViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SearchFilterPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as ISearchPageViewModel;
        }

        private void SearchButton_Tapped(object sender, TappedRoutedEventArgs e) {}
        private void ClearButton_Tapped(object sender, TappedRoutedEventArgs e) => ViewModel.Filter.Clear();
    }

    public class SearchFilterPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SettingsPageTemplate { get; set; }
        public DataTemplate SinglePageTemplate { get; set; }
        public DataTemplate MultiPageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISettingsPageViewModel => SettingsPageTemplate,
            ISearchPageViewModel vm => vm.Headers.Count() switch
            {
                0 => null,
                1 => SinglePageTemplate,
                _ => MultiPageTemplate,
            },
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }

}
