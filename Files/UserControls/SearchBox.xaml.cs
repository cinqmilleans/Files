using Files.Filesystem;
using Files.Filesystem.Search;
using Files.ViewModels;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class SearchBox : UserControl
    {
        public SearchBoxViewModel SearchBoxViewModel
        {
            get => (SearchBoxViewModel)GetValue(SearchBoxViewModelProperty);
            set => SetValue(SearchBoxViewModelProperty, value);
        }

        // Using a DependencyProperty as the backing store for SearchBoxViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchBoxViewModelProperty =
            DependencyProperty.Register(nameof(SearchBoxViewModel), typeof(SearchBoxViewModel), typeof(SearchBox), new PropertyMetadata(null));

        public SearchBox()
        {
            InitializeComponent();
        }

        private void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
            => SearchBoxViewModel.SearchRegion_TextChanged(sender, e);

        private void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
            => SearchBoxViewModel.SearchRegion_QuerySubmitted(sender, e);

        private void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
            => SearchBoxViewModel.SearchRegion_SuggestionChosen(sender, e);

        private void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
            => SearchBoxViewModel.SearchRegion_Escaped(sender, e);

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedValue is ISearchOption option)
            {
                SearchBoxViewModel.OptionSelected(option);
                SearchRegion.Focus(FocusState.Programmatic);
            }
        }

        private void Popup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //var a = SearchRegion.FindDescendant("TextBox") as TextBox;
            //var b = SearchRegion.FindDescendant("Popup") as Popup;
            //var c = SearchRegion.FindDescendant("SuggestionsPopup") as Popup;

            //c.VerticalOffset = b.ActualHeight + a.ActualHeight;
        }

        private void Popup_Opened(object sender, object e)
        {

        }
    }

    public class SuggestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OptionKeyTemplate { get; set; }
        public DataTemplate OptionTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ISearchOptionKey)
            {
                return OptionKeyTemplate;
            }
            if (item is ISearchOption)
            {
                return OptionTemplate;
            }
            if (item is ListedItem)
            {
                return ItemTemplate;
            }
            throw new ArgumentException();
        }
    }

    public class SearchOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ISearchOptionKey optionKey)
            {
                return $"{optionKey.Text}:";
            }
            if (value is ISearchOption option)
            {
                return $"{option.Key.Text}:{option.Text}";
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
