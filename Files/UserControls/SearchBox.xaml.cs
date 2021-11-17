using Files.Filesystem;
using Files.Filesystem.Search;
using Files.ViewModels;
using Files.ViewModels.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class SearchBox : UserControl
    {
        public static readonly DependencyProperty SearchBoxViewModelProperty =
            DependencyProperty.Register(nameof(SearchBoxViewModel), typeof(SearchBoxViewModel), typeof(SearchBox), new PropertyMetadata(null));

        public SearchBoxViewModel SearchBoxViewModel
        {
            get => (SearchBoxViewModel)GetValue(SearchBoxViewModelProperty);
            set => SetValue(SearchBoxViewModelProperty, value);
        }

        public SearchBox() => InitializeComponent();

        private void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
            => SearchBoxViewModel.SearchRegion_TextChanged(sender, e);
        private void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
            => SearchBoxViewModel.SearchRegion_QuerySubmitted(sender, e);
        private void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
            => SearchBoxViewModel.SearchRegion_SuggestionChosen(sender, e);
        private void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
            => SearchBoxViewModel.SearchRegion_Escaped(sender, e);
    }

    public class SuggestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SyntaxTemplate { get; set; }
        public DataTemplate ParserTemplate { get; set; }
        public DataTemplate ResultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            IParserSyntax => SyntaxTemplate,
            IFilterParser => ParserTemplate,
            ListedItem => ResultTemplate,
            _ => null,
        };
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }

    public class SyntaxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate ParameterTemplate { get; set; }
        public DataTemplate DescribedParameterTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ITextSyntaxItem => TextTemplate,
            IParameterSyntaxItem syntaxItem => string.IsNullOrEmpty(syntaxItem.Description) ? ParameterTemplate : DescribedParameterTemplate,
            _ => null,
        };
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }

}
