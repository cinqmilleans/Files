using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Linq;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPage : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchFilterViewModel), typeof(SearchFilterPage), new PropertyMetadata(null));

        private ISearchFilterViewModel ViewModel
        {
            get => (ISearchFilterViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SearchFilterPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as ISearchFilterViewModel;
        }
    }

    public class SearchFilterPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SettingsPageTemplate { get; set; }
        public DataTemplate SinglePageTemplate { get; set; }
        public DataTemplate MultiPageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISettingsFilterViewModel => SettingsPageTemplate,
            ISearchFilterViewModel vm => vm.Alternatives.Count() switch
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
