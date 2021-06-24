using Files.Extensions;
using Files.Filesystem;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.ViewModels
{
    public class SearchBoxViewModel : ObservableObject, ISearchBox
    {
        private readonly ISearchOptionFactory optionFactory = SearchOptionFactory.Default;

        public ISearchOptionKey[] OptionKeys { get; } = SearchOptionFactory.Default.AllKeys.Values.ToArray();

        private ISearchOption currentOption;
        public ISearchOption CurrentOption
        {
            get => currentOption;
            set
            {
                SetProperty(ref currentOption, value);
                OnPropertyChanged(nameof(Suggestions));
            }
        }

        private string query;
        public string Query
        {
            get => query;
            set
            {
                SetProperty(ref query, value);
                UpdateSelectedOption();
            }
        }

        private void UpdateSelectedOption()
        {
            int space = query.LastIndexOf(' ');
            string item = space < 0 ? query : query.Substring(space + 1);

            UpdateSuggestions(optionFactory.GetOptionKeySuggestions(item));
            UpdateSuggestions(optionFactory.GetOptionSuggestions(item));
        }

        public void OptionSelected(ISearchOptionKey optionKey)
        {
            string query = Query.Trim();
            string space = query.Length > 0 ? " " : string.Empty;
            Query = $"{query}{space}{optionKey.Text}:";
        }

        public event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        public event TypedEventHandler<ISearchBox, SearchBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<ISearchBox> Escaped;

        private readonly SuggestionComparer suggestionComparer = new SuggestionComparer();

        private readonly ObservableCollection<object> emptyCollection = new ObservableCollection<object>();
        private readonly ObservableCollection<object> suggestions = new ObservableCollection<object>();
        public ObservableCollection<object> Suggestions => currentOption is null ? suggestions : emptyCollection;

        public void ClearSuggestions() => ClearSuggestions<ListedItem>();
        public void SetSuggestions(IEnumerable<ListedItem> suggestions) => UpdateSuggestions(suggestions);

        private void ClearSuggestions<T>()
        {
            this.suggestions
                .Where(suggestion => suggestion is T)
                .ToList()
                .ForEach(suggestion => Suggestions.Remove(suggestion));
        }
        private void UpdateSuggestions<T>(IEnumerable<T> suggestions)
        {
            var oldItems = this.suggestions.Where(suggestion => suggestion is T).ToList();
            var newItems = suggestions.OrderBy(suggestion => suggestion, suggestionComparer).Cast<object>().ToList();

            oldItems.Except(newItems, suggestionComparer).ForEach(suggestion => this.suggestions.Remove(suggestion));
            newItems.Except(oldItems, suggestionComparer).ForEach(suggestion => Insert(suggestion));

            void Insert(object suggestion)
            {
                var indexSuggestion = this.suggestions.FirstOrDefault(oldSuggestion => suggestionComparer.Compare(oldSuggestion, suggestion) < 1);
                if (!(indexSuggestion is null))
                {
                    int index = this.suggestions.IndexOf(indexSuggestion);
                    this.suggestions.Insert(index, suggestion);
                }
                else
                {
                    this.suggestions.Add(suggestion);
                }
            }
        }

        public void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            int space = query.LastIndexOf(' ');
            string item = space < 0 ? query : query.Substring(space + 1);
            if (item.Contains(':') && optionFactory.CanProvide(item))
            {
                CurrentOption = optionFactory.Provide(item);
            }
            else
            {
                TextChanged?.Invoke(this, new SearchBoxTextChangedEventArgs(e.Reason));
            }
        }

        public void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            QuerySubmitted?.Invoke(this, new SearchBoxQuerySubmittedEventArgs(e.ChosenSuggestion as ListedItem));
        }

        public void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            if (e.SelectedItem is ListedItem listedItem)
            {
                SuggestionChosen?.Invoke(this, new SearchBoxSuggestionChosenEventArgs(listedItem));
            }
        }

        public void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            Escaped?.Invoke(this, this);
        }

        public class SuggestionComparer : IEqualityComparer<object>, IComparer<object>
        {
            public int Compare(object x, object y) => ToValue(y).CompareTo(ToValue(x));
            public new bool Equals(object x, object y) => ToValue(y).Equals(ToValue(x));
            public int GetHashCode(object o) => ToValue(o).GetHashCode();

            private static (ushort, string) ToValue (object o)
            {
                if (o is ISearchOptionKey optionKey)
                {
                    return (1, $"{optionKey.Text}");
                }
                if (o is ISearchOption option)
                {
                    return (2, $"{option.Key.Text}:{option.Text}");
                }
                if (o is ListedItem item)
                {
                    return (3, item.ItemPath);
                }
                throw new ArgumentException();
            }
        }
    }
}
