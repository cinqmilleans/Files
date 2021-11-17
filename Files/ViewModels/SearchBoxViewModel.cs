using Files.Extensions;
using Files.Filesystem;
using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp.UI;
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
        private readonly IFilterParserFactory parserFactory = new FilterParserFactory();

        private string query;
        public string Query
        {
            get => query;
            set => SetProperty(ref query, value);
        }

        public event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        public event TypedEventHandler<ISearchBox, SearchBoxSuggestionChosenEventArgs> SuggestionChosen;
        public event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        public event EventHandler<ISearchBox> Escaped;

        private readonly SuggestionComparer suggestionComparer = new SuggestionComparer();

        public ObservableCollection<object> Suggestions { get; } = new ObservableCollection<object>();

        public void ClearSuggestions() => ClearSuggestions<ListedItem>();
        public void SetSuggestions(IEnumerable<ListedItem> suggestions) => SetSuggestions<ListedItem>(suggestions);

        public void SearchRegion_TextChanged(AutoSuggestBox s, AutoSuggestBoxTextChangedEventArgs e)
        {
            var item = GetQueryItem(s.FindDescendant<TextBox>());

            var parserNames = parserFactory.Names;

            var syntaxSuggestions = item.Contains(':')
                ? parserNames.Where(name => item.StartsWith(name + ':')).Select(name => new ParserSyntax(parserFactory.GetParser(name)))
                : Enumerable.Empty<IParserSyntax>();
            SetSuggestions(syntaxSuggestions);

            if (syntaxSuggestions.Any())
            {
                ClearSuggestions<IFilterParser>();
                ClearSuggestions<ListedItem>();
            }

            var keySuggestions = item.Length >= 2
                ? parserNames.Where(name => name.StartsWith(item)).Select(name => parserFactory.GetParser(name))
                : Enumerable.Empty<IFilterParser>();
            SetSuggestions(keySuggestions);

            TextChanged?.Invoke(this, new SearchBoxTextChangedEventArgs(e.Reason));
        }
        public void SearchRegion_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            //SuggestionChosen?.Invoke(this, new SearchBoxSuggestionChosenEventArgs(listedItem));
        }
        public void SearchRegion_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            if (e.ChosenSuggestion is IFilterParser parser)
            {
                SetQueryItem(sender.FindDescendant<TextBox>(), string.Format("{0}:", parser.Name));
            }
            else if (e.ChosenSuggestion is ListedItem item)
            {
                QuerySubmitted?.Invoke(this, new SearchBoxQuerySubmittedEventArgs(item));
            }
        }

        public void SearchRegion_Escaped(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs e)
        {
            Escaped?.Invoke(this, this);
        }

        private void ClearSuggestions<T>() => SetSuggestions(Enumerable.Empty<T>());
        private void SetSuggestions<T>(IEnumerable<T> suggestions)
        {
            var news = suggestions.OrderBy(suggestion => suggestion, suggestionComparer).ToList();
            var olds = Suggestions.OfType<T>().ToList();

            olds.Except(news).ForEach(suggestion => Suggestions.Remove(suggestion));
            news.Except(olds).ForEach(suggestion => Insert(suggestion));

            void Insert(T newSuggestion)
            {
                var indexSuggestion = Suggestions.FirstOrDefault(suggestion => suggestionComparer.Compare(suggestion, newSuggestion) < 1);
                if (!(indexSuggestion is null))
                {
                    int index = Suggestions.IndexOf(indexSuggestion);
                    Suggestions.Insert(index, newSuggestion);
                }
                else
                {
                    Suggestions.Add(newSuggestion);
                }
            }
        }

        private static string GetQueryItem(TextBox box)
        {
            if (box.SelectionLength > 0)
            {
                return string.Empty;
            }

            string item = box.Text;
            int position = box.SelectionStart;

            int index = item.Substring(0, position).LastIndexOf(' ');
            if (index >= 0)
            {
                item = item.Substring(index + 1);
            }

            index = item.IndexOf(' ');
            if (index >= 0)
            {
                item = item.Substring(0, index);
            }

            return item;
        }
        private void SetQueryItem(TextBox box, string item)
        {
            Query = item ?? string.Empty;
        }

        private class SuggestionComparer : IEqualityComparer<object>, IComparer<object>
        {
            public int Compare(object x, object y) => GetKey(y).CompareTo(GetKey(x));
            public new bool Equals(object x, object y) => GetKey(y).Equals(GetKey(x));
            public int GetHashCode(object o) => GetKey(o).GetHashCode();

            private static (ushort, string) GetKey(object o) => o switch
            {
                IParserSyntax syntax => (1, syntax.Name),
                IFilterParser key => (2, key.Name),
                ListedItem item => (3, item.ItemPath),
                _ => throw new ArgumentException(),
            };
        }
    }
}
