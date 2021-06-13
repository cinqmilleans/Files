using Files.Filesystem;
using Files.Filesystem.Search;
using Files.Helpers.XamlHelpers;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI;
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
        public SearchBox()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            queryTextBox.Text = string.Empty;
            optionViewModel.Visibles.Clear();
            optionViewModel.Hiddens.Clear();
            optionViewModel.HasHiddenVisibility = Visibility.Collapsed;
            optionSizes.Clear();
        }

        #region event
        public event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        public event TypedEventHandler<ISearchBox, SearchBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<ISearchBox> Escaped;

        private void SearchRegion_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOptionsVisibility();
        }
        private void VisibleOptionControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateOptionsVisibility();
        }

        private void SearchRegion_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space || e.Key == VirtualKey.Enter || e.Key == VirtualKey.Tab)
            {
                var field = ReadField();
                if (field.Contains(':'))
                {
                    var parts = field.Split(':', 2);
                    if (optionViewModel.OptionKeys.ContainsKey(parts[0]))
                    {
                        var key = optionViewModel.OptionKeys[parts[0]];
                        if (key.Format.CanParseValue(parts[1]))
                        {
                            var value = key.Format.ParseValue(parts[1]);
                            var option = new SearchOption { Key = key, Value = value };
                            ClearField();
                            AddOption(option);
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
                            ReplaceField($"{optionKey.Text}:");
                            e.Handled = true;
                        }
                        if (data is ISearchOption option)
                        {
                            ClearField();
                            AddOption(option);
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
                string field = ReadField();
                if ((field.EndsWith('<') || field.EndsWith('>')) && !field.Contains(':') && field.Length > 1)
                {
                    ReplaceField(field.Insert(field.Length - 1, ":"));
                    return;
                }

                string queryText = (queryTextBox.Text + " " + ProvideFilter()).Trim();
                UpdateSuggestions();
                TextChanged?.Invoke(this, new SearchBoxTextChangedEventArgs(queryText));
            }
        }
        private void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (Suggestions.Count == 1 && e.SelectedItem is ISearchSuggestion suggestion)
            {
                if (suggestion.Data is ISearchOptionKey optionKey)
                {
                    ReplaceField($"{optionKey.Text}:");
                    UpdateSuggestions();
                    SearchRegion.Focus(FocusState.Programmatic);
                }
                else if (suggestion.Data is ISearchOption option)
                {
                    ClearField();
                    AddOption(option);
                    ReplaceField($"{option.Key.Text}:{option.Value.Text}");
                    SearchRegion.Focus(FocusState.Programmatic);
                }
                else if (suggestion.Data is ListedItem item)
                {
                    SuggestionChosen?.Invoke(this, new SearchBoxSuggestionChosenEventArgs(item));
                }
            }
        }
        private void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion is OptionKeySearchSuggestion)
            {
                UpdateSuggestions();
                SearchRegion.Focus(FocusState.Programmatic);
            }
            else if (e.ChosenSuggestion is OptionSearchSuggestion option)
            {
                AddOption(option.Data);
                ClearField();
                SearchRegion.Focus(FocusState.Programmatic);
            }
            else if (e.ChosenSuggestion is ListedItem item)
            {
                SuggestionChosen?.Invoke(this, new SearchBoxSuggestionChosenEventArgs(item));
            }
            else
            {
                string query = (queryTextBox.Text + " " + ProvideFilter()).Trim();
                QuerySubmitted?.Invoke(this, new SearchBoxQuerySubmittedEventArgs(query));
            }
        }

        private void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            Escaped?.Invoke(this, this);
        }

        private void TemplateControl_Loaded(object sender, RoutedEventArgs e)
        {
            var template = sender as FrameworkElement;
            queryTextBox = DependencyObjectHelpers.FindChild<TextBox>(SearchRegion);
            visibleOptionControl = template.FindDescendant("VisibleOptionControl") as Control;
            template.FindDescendant("OptionKeyButton").DataContext = optionViewModel;
            visibleOptionControl.DataContext = optionViewModel;
            template.FindDescendant("HiddenOptionButton").DataContext = optionViewModel;
        }

        private void OptionKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var optionKey = (sender as Control).DataContext as ISearchOptionKey;
            var field = ReadField();
            if (field.Contains(':'))
            {
                ReplaceField($"{optionKey.Text}:");
            }
            else
            {
                AddField($"{optionKey.Text}:");
            }
            SearchRegion.Focus(FocusState.Programmatic);
            UpdateSuggestions();
        }

        private void VisibleOptionItem_Loaded(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var option = element.DataContext as ISearchOption;
            if (!optionSizes.ContainsKey(option))
            {
                optionSizes.Add(option, element.ActualWidth + 4);
            }
        }

        private void CloseOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var option = (sender as FrameworkElement).DataContext as ISearchOption;
            RemoveOption(option);
            UpdateOptionsVisibility();
            SearchRegion.Focus(FocusState.Programmatic);
        }
        #endregion

        #region field
        private TextBox queryTextBox;

        private readonly Regex fieldRegex = new Regex(@"[^\s]+");

        private void ClearField() => ReplaceField(string.Empty);

        private string ReadField()
        {
            var query = queryTextBox.Text;
            if (query.Length == 0 || char.IsWhiteSpace(query.Last()))
            {
                return string.Empty;
            }
            return fieldRegex.Matches(query).LastOrDefault()?.Value ?? string.Empty;
        }

        private void ReplaceField(string text)
        {
            string field = ReadField();
            queryTextBox.Text = queryTextBox.Text.Substring(0, queryTextBox.Text.Length - field.Length) + text;
            queryTextBox.Select(queryTextBox.Text.Length, 0);
        }
        private void AddField(string text)
        {
            var query = queryTextBox.Text;
            string space = query.Length == 0 || !char.IsWhiteSpace(query.Last()) ? " " : string.Empty;
            queryTextBox.Text += space + text;
            queryTextBox.Select(queryTextBox.Text.Length, 0);
        }
        #endregion

        #region option
        private const double minQuerySize = 200;

        private Control visibleOptionControl;

        private readonly OptionViewModel optionViewModel = new OptionViewModel();

        private readonly SearchOptionComparer optionComparer = new SearchOptionComparer();

        private readonly IDictionary<ISearchOption, double> optionSizes = new Dictionary<ISearchOption, double>();

        public string ProvideFilter() =>
            string.Join(' ', optionViewModel.Visibles.Concat(optionViewModel.Hiddens).Select(option => option.Key.ProvideFilter(option.Value)));

        private void RemoveOption(ISearchOption option)
        {
            optionSizes.Remove(option);
            if (optionViewModel.Visibles.Contains(option, optionComparer))
            {
                optionViewModel.Visibles.Remove(option);
            }
            if (optionViewModel.Hiddens.Contains(option, optionComparer))
            {
                optionViewModel.Hiddens.Remove(option);
            }
        }
        private void AddOption(ISearchOption option)
        {
            if (!optionViewModel.Visibles.Any() || !optionComparer.Equals(optionViewModel.Visibles[0], option))
            {
                RemoveOption(option);
                optionViewModel.Visibles.Insert(0, option);
            }
        }

        private void ShowOption()
        {
            if (optionViewModel.Hiddens.Any())
            {
                var option = optionViewModel.Hiddens.First();
                optionViewModel.Hiddens.RemoveAt(0);
                optionViewModel.Visibles.Add(option);
                optionViewModel.HasHiddenVisibility = optionViewModel.Hiddens.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void HideOption()
        {
            if (optionViewModel.Visibles.Any())
            {
                var option = optionViewModel.Visibles.Last();
                optionViewModel.Visibles.RemoveAt(optionViewModel.Visibles.Count - 1);
                optionViewModel.Hiddens.Insert(0, option);
                optionViewModel.HasHiddenVisibility = optionViewModel.Hiddens.Any() ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateOptionsVisibility()
        {
            if (visibleOptionControl is null)
            {
                return;
            }
            if (visibleOptionControl.ActualWidth > SearchRegion.ActualWidth - 3 * 34 - minQuerySize)
            {
                HideOption();
            }
            if (optionViewModel.Hiddens.Any() && optionSizes.ContainsKey(optionViewModel.Hiddens[0]) &&
                visibleOptionControl.ActualWidth < SearchRegion.ActualWidth - 3 * 34 - minQuerySize - optionSizes[optionViewModel.Hiddens[0]])
            {
                ShowOption();
            }
        }

        private class OptionViewModel : ObservableObject
        {
            public IReadOnlyDictionary<string, ISearchOptionKey> OptionKeys { get; } =
                SearchOptionKeyProvider.Default.ProvideAllKeys().ToDictionary(key => key.Text);

            public ObservableCollection<ISearchOption> Visibles { get; } = new ObservableCollection<ISearchOption>();
            public ObservableCollection<ISearchOption> Hiddens { get; } = new ObservableCollection<ISearchOption>();

            public ISearchOptionKey[] OptionKeyValues { get; }

            private Visibility hasHiddenVisibility = Visibility.Collapsed;
            public Visibility HasHiddenVisibility
            {
                get => hasHiddenVisibility;
                set => SetProperty(ref hasHiddenVisibility, value);
            }

            public OptionViewModel()
            {
                OptionKeyValues = OptionKeys.Values.OrderBy(key => key.Text).ToArray();
            }
        }
        #endregion

        #region suggestion
        public ObservableCollection<ISearchSuggestion> Suggestions { get; } = new ObservableCollection<ISearchSuggestion>();

        private readonly SuggestionComparer suggestionComparer = new SuggestionComparer();

        public void ClearSuggestions() => ClearSuggestions<ListedItem>();

        public void SetSuggestions(IEnumerable<ListedItem> items)
        {
            var query = queryTextBox.Text;
            var suggestions = items.Select(item => new ItemSearchSuggestion(item, query));
            SetSuggestions<ListedItem>(suggestions);
        }

        private void UpdateSuggestions()
        {
            var field = ReadField();

            SetSuggestions<ISearchOptionKey>(GetOptionKeySuggestions(field));
            SetSuggestions<ISearchOption>(GetOptionSuggestions(field));
        }
        private IEnumerable<ISearchSuggestion> GetOptionKeySuggestions(string field)
        {
            var query = queryTextBox.Text;
            if (field.Length == 0)
            {
                return new ISearchSuggestion[0];
            }
            var lowerText = field.ToLower();
            string prefix = query.Substring(0, query.Length - field.Length);

            return optionViewModel.OptionKeys.Values
                .OrderBy(key => key.Text)
                .Where(key => key.Text.StartsWith(field))
                .Select(key => new OptionKeySearchSuggestion(key, $"{prefix}{key.Text}:"));
        }
        private IEnumerable<ISearchSuggestion> GetOptionSuggestions(string field)
        {
            var query = queryTextBox.Text;
            if (field.Contains(':'))
            {
                var keyText = field.Split(':')[0].ToLower();
                if (optionViewModel.OptionKeys.ContainsKey(keyText))
                {
                    string prefix = query.Substring(0, query.Length - field.Length);

                    return optionViewModel.OptionKeys[keyText].ToSuggestions()
                        .Select(option => new OptionSearchSuggestion(option, $"{prefix}{option.Key.Text}:{option.Value.Text}"));
                }
            }
            return new ISearchSuggestion[0];
        }

        private void ClearSuggestions<T>()
        {
            var oldSuggestions = Suggestions.Where(suggestion => suggestion.Data is T).ToList();
            oldSuggestions.ForEach(oldSuggestion => Suggestions.Remove(oldSuggestion));
        }
        private void SetSuggestions<T>(IEnumerable<ISearchSuggestion> suggestions)
        {
            var insertSuggestions = suggestions.OrderBy(suggestion => suggestion, suggestionComparer).ToList();
            var currentSuggestions = Suggestions.Where(suggestion => suggestion.Data is T).ToList();

            var oldSuggestions = currentSuggestions.Except(insertSuggestions, suggestionComparer).ToList();
            oldSuggestions.ForEach(oldSuggestion => Suggestions.Remove(oldSuggestion));

            var newSuggestions = insertSuggestions.Except(currentSuggestions, suggestionComparer).ToList();
            newSuggestions.ForEach(newSuggestion => Add(newSuggestion));

            void Add(ISearchSuggestion newSuggestion)
            {
                var indexSuggestion = Suggestions.FirstOrDefault(suggestion => suggestionComparer.Compare(suggestion, newSuggestion) > 1);
                if (indexSuggestion is null)
                {
                    Suggestions.Add(newSuggestion);
                }
                else
                {
                    int index = Suggestions.IndexOf(indexSuggestion);
                    Suggestions.Insert(index, newSuggestion);
                }
            }
        }
        #endregion
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
                return $"{option.Key.Text}:{option.Value.Text}";
            }
            throw new ArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
