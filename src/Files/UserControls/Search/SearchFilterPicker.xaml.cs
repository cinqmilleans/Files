using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPicker : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchFilter), typeof(SearchFilterPicker), new PropertyMetadata(null));

        public ISearchFilter ViewModel
        {
            get => (ISearchFilter)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SearchFilterPicker() => InitializeComponent();

        private void OpenButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var navigator = Ioc.Default.GetService<ISearchNavigator>();
            var filter = (sender as FrameworkElement).DataContext as ISearchFilter;
            navigator.GoPage(filter);
        }
    }

    public class SearchFilterPickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GroupTemplate { get; set; }
        public DataTemplate SizeRangeTemplate { get; set; }
        public DataTemplate DateRangeTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISearchFilterCollection => GroupTemplate,
            ISizeRangeFilter => SizeRangeTemplate,
            IDateRangeFilter => DateRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
