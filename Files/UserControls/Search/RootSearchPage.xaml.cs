using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files.UserControls.Search
{
    public sealed partial class RootSearchPage : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(IRootSearchPageViewModel), typeof(RootSearchPage), new PropertyMetadata(null));

        public IRootSearchPageViewModel ViewModel
        {
            get => (IRootSearchPageViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public RootSearchPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as IRootSearchPageViewModel;
        }
    }
}
