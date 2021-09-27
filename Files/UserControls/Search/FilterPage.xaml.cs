using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files.UserControls.Search
{
    public sealed partial class FilterPage : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(IFilterViewModel), typeof(FilterPage), new PropertyMetadata(null));

        public IFilterViewModel ViewModel
        {
            get => (IFilterViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public FilterPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as IFilterViewModel;
        }

        private void TitleButton_Click(object sender, RoutedEventArgs e) => NavigatorViewModel.Default.Back();
    }
}
