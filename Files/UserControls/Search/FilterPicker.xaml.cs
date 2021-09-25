using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class FilterPicker : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(IFilterViewModel), typeof(FilterPicker), new PropertyMetadata(null));

        public IFilterViewModel ViewModel
        {
            get => (IFilterViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public FilterPicker() => InitializeComponent();
    }

    public class FilterPickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate OperatorTemplate { get; set; }
        public DataTemplate DateRangeTemplate { get; set; }
        public DataTemplate SizeRangeTemplate { get; set; }
        public DataTemplate KindTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            FilterViewModelCollection => CollectionTemplate,
            OperatorViewModel => OperatorTemplate,
            IDateRangeViewModel _ => DateRangeTemplate,
            ISizeRangeViewModel _ => SizeRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
