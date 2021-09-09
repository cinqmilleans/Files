using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files.UserControls.Search
{
    public sealed partial class SettingSearchPage : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISizeRangePageViewModel), typeof(SettingSearchPage), new PropertyMetadata(null));

        public ISizeRangePageViewModel ViewModel
        {
            get => (ISizeRangePageViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SettingSearchPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as ISizeRangePageViewModel;
        }
    }

    public class SettingSearchPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SizeRangeTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISizeRangePageViewModel _ => SizeRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
