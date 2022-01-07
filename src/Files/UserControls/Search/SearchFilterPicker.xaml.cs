using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPicker : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchFilterViewModel), typeof(SearchFilterPicker), new PropertyMetadata(null));

        public ISearchFilterViewModel ViewModel
        {
            get => (ISearchFilterViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SearchFilterPicker() => InitializeComponent();
    }

    public class SearchFilterPickerTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
