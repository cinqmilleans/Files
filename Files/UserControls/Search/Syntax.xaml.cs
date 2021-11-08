using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class Syntax : Page
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISyntaxViewModel), typeof(Picker), new PropertyMetadata(null));

        public ISyntaxViewModel ViewModel
        {
            get => (ISyntaxViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }


        public Syntax() => InitializeComponent();
    }

    public class SyntaxTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DateRangeTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            IDateRangeSyntaxViewModel => DateRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
