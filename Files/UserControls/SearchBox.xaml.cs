using Files.Filesystem;
using Files.Filesystem.Search;
using Files.Helpers.XamlHelpers;
using Files.ViewModels.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class SearchBox : UserControl, ISearchBox
    {
        public event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        public event TypedEventHandler<ISearchBox, SearchBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<ISearchBox> Escaped;

        public static readonly DependencyProperty QueryProperty =
            DependencyProperty.Register("Query", typeof(string), typeof(SearchBox), new PropertyMetadata(string.Empty));

        public string Query
        {
            get => (string)GetValue(QueryProperty);
            set => SetValue(QueryProperty, value ?? string.Empty);
        }

        private IDictionary<string, ISearchOptionKey> AllKeys { get; } =
            SearchOptionKeyProvider.Default.ProvideAllKeys().ToDictionary(key => key.Text);

        private Field field = new Field();
        private OptionCollection Options { get; } = new OptionCollection();
        private ISearchSuggestionCollection Suggestions { get; } = new SearchSuggestionCollection();

        public SearchBox()
        {
            InitializeComponent();
        }

        public void ClearSuggestions() => Suggestions.Clear<ListedItem>();
        public void SetSuggestions(IEnumerable<ListedItem> suggestions)
        {
            var items = suggestions.Select(i => new ItemSearchSuggestion(i, Query));
            Suggestions.Set<ListedItem>(items);
        }

        private void SearchRegion_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Enter || e.Key == VirtualKey.Tab)
            {
                var text = field.Read();
                if (text.Contains(':'))
                {
                    var parts = text.Split(':', 2);
                    if (AllKeys.ContainsKey(parts[0]))
                    {
                        var key = AllKeys[parts[0]];
                        if (key.Format.CanParseValue(parts[1]))
                        {
                            var value = key.Format.ParseValue(parts[1]);
                            var option = new SearchOption { Key = key, Value = value };
                            field.Clear();
                            Options.Add(option);
                            e.Handled = true;
                        }
                    }
                }
                else
                {
                    if (e.Key != VirtualKey.Space && Suggestions.Any())
                    {
                        var data = Suggestions.ElementAt(0).Data;
                        if (data is ISearchOptionKey optionKey)
                        {
                            field.Replace($"{optionKey.Text}:");
                            e.Handled = true;
                        }
                        if (data is ISearchOption option)
                        {
                            field.Clear();
                            Options.Add(option);
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                UpdateOptionSuggestions();
                TextChanged?.Invoke(this, new SearchBoxTextChangedEventArgs(e.Reason));
            }
        }
        private void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (Suggestions.Count() == 1 && e.SelectedItem is ISearchSuggestion suggestion)
            {
                if (suggestion.Data is ISearchOptionKey optionKey)
                {
                    field.Replace($"{optionKey.Text}:");
                    UpdateOptionSuggestions();
                }
                if (suggestion.Data is ISearchOption option)
                {
                    field.Replace($"{option.Key.Text}:{option.Value.Text}");
                }
                if (suggestion.Data is ListedItem item)
                {
                    SuggestionChosen?.Invoke(this, new SearchBoxSuggestionChosenEventArgs(item));
                }
            }
        }
        private void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion is OptionKeySearchSuggestion)
            {
                UpdateOptionSuggestions();
            }
            else if (e.ChosenSuggestion is OptionSearchSuggestion option)
            {
                Options.Add(option.Data);
                field.Clear();
            }
            else
            {
                var item = e.ChosenSuggestion is ItemSearchSuggestion suggestion ? suggestion.Data : null;
                string query = (Query + " " + Options.ProvideFilter()).Trim();
                QuerySubmitted?.Invoke(this, new SearchBoxQuerySubmittedEventArgs(query, item));
            }
        }

        private void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            Escaped?.Invoke(this, this);
        }

        private void OptionKeyControl_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ItemsControl).ItemsSource = AllKeys.Values;
        }
        private void OptionControl_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ItemsControl).ItemsSource = Options.VisibleOptions;
            field.TextBox = DependencyObjectHelpers.FindChild<TextBox>(SearchRegion);
        }
        private void OptionKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var optionKey = (sender as Control).DataContext as ISearchOptionKey;
            field.Add($"{optionKey.Text}:");
            SearchRegion.Focus(FocusState.Programmatic);
        }

        private void UpdateOptionSuggestions()
        {
            var fieldText = field.Read();

            Suggestions.Set<ISearchOptionKey>(GetOptionKeySuggestions(fieldText));
            Suggestions.Set<ISearchOption>(GetOptionSuggestions(fieldText));
        }

        private IEnumerable<ISearchSuggestion> GetOptionKeySuggestions(string fieldText)
        {
            if (fieldText.Length == 0)
            {
                return new ISearchSuggestion[0];
            }
            var lowerText = fieldText.ToLower();
            string prefix = Query.Substring(0, Query.Length - fieldText.Length);
            return AllKeys.Values
                .Where(key => key.Text.StartsWith(fieldText))
                .Select(key => new OptionKeySearchSuggestion(key, $"{prefix}{key.Text}:"));
        }
        private IEnumerable<ISearchSuggestion> GetOptionSuggestions(string fieldText)
        {
            if (fieldText.Contains(':'))
            {
                var keyText = fieldText.Split(':')[0].ToLower();
                if (AllKeys.ContainsKey(keyText))
                {
                    string prefix = Query.Substring(0, Query.Length - fieldText.Length);

                    return AllKeys[keyText].ToSuggestions()
                        .Select(option => new OptionSearchSuggestion(option, $"{prefix}{option.Key.Text}:{option.Value.Text}"));
                }
            }
            return new ISearchSuggestion[0];
        }

        private class Field
        {
            public TextBox TextBox { get; set; }

            private readonly Regex regex = new Regex(@"[^\s]+");

            public void Clear() => Replace(string.Empty);

            public string Read()
            {
                var text = TextBox.Text ?? string.Empty;
                if (text.Length == 0 || char.IsWhiteSpace(text.Last()))
                {
                    return string.Empty;
                }
                return regex.Matches(TextBox.Text).LastOrDefault()?.Value ?? string.Empty;
            }
            public void Replace(string text)
            {
                string field = Read();
                TextBox.Text = TextBox.Text.Substring(0, TextBox.Text.Length - field.Length) + text;
                TextBox.Select(TextBox.Text.Length, 0);
            }
            public void Add(string text)
            {
                string space = TextBox.Text.Length == 0 || !char.IsWhiteSpace(TextBox.Text.Last()) ? " " : string.Empty;
                TextBox.Text += text;
                TextBox.Select(TextBox.Text.Length, 0);
            }
        }

        private class OptionCollection
        {
            private readonly SearchOptionComparer comparer = new SearchOptionComparer();

            private readonly ObservableCollection<ISearchOption> visibleOptions = new ObservableCollection<ISearchOption>();
            private readonly ObservableCollection<ISearchOption> hiddenOptions = new ObservableCollection<ISearchOption>();

            public IEnumerable<ISearchOption> VisibleOptions => visibleOptions;
            public IEnumerable<ISearchOption> HiddenOptions => hiddenOptions;

            public void Remove(ISearchOption option)
            {
                if (visibleOptions.Contains(option, comparer))
                {
                    visibleOptions.Remove(option);
                }
                if (hiddenOptions.Contains(option, comparer))
                {
                    hiddenOptions.Remove(option);
                }
            }

            public void Add(ISearchOption option)
            {
                if (visibleOptions.Count == 0 || !comparer.Equals(visibleOptions[0], option))
                {
                    Remove(option);
                    visibleOptions.Insert(0, option);
                }
            }

            public string ProvideFilter()
                => string.Join(' ', VisibleOptions.Concat(hiddenOptions).Select(option => option.Key.ProvideFilter(option.Value)));
        }
    }

    public class SuggestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OptionKeyTemplate { get; set; }
        public DataTemplate OptionTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is OptionKeySearchSuggestion)
            {
                return OptionKeyTemplate;
            }
            if (item is OptionSearchSuggestion)
            {
                return OptionTemplate;
            }
            if (item is ItemSearchSuggestion)
            {
                return ItemTemplate;
            }
            throw new ArgumentException();
        }
    }

    public class SearchOptionKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && !s.EndsWith(':'))
            {
                return $"{s}:";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
