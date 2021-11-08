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

        public ObservableCollection<ListedItem> Suggestions { get; } = new ObservableCollection<ListedItem>();

        public void ClearSuggestions()
        {
            Suggestions.Clear();
        }

        public void SetSuggestions(IEnumerable<ListedItem> suggestions)
        {
            var items = suggestions.OrderBy(suggestion => suggestion, suggestionComparer).ToList();

            var oldSuggestions = Suggestions.Except(items, suggestionComparer).ToList();
            foreach (var oldSuggestion in oldSuggestions)
            {
                Suggestions.Remove(oldSuggestion);
            }

            var newSuggestions = items.Except(Suggestions, suggestionComparer).ToList();
            foreach (var newSuggestion in newSuggestions)
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

        private readonly IMainFilterParser filterParser = new MainFilterParser();

        public void SearchRegion_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs e)
        {
            if (Query == "date")
            {

            }


            TextChanged?.Invoke(this, new SearchBoxTextChangedEventArgs(e.Reason));
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
            public int Compare(object x, object y) => GetKey(y).CompareTo(GetKey(x));
            public new bool Equals(object x, object y) => GetKey(y).Equals(GetKey(x));
            public int GetHashCode(object o) => GetKey(o).GetHashCode();

            private static string GetKey(object o) => o switch
            {
                ParserSuggestion suggestion => string.Empty,
                ListedItem item => item.ItemPath,
                _ => throw new ArgumentException(),
            };
        }

        public interface IParserSuggestion
        {
            string Name { get; }
            string Description { get; }

            string
        }

        public interface IParserSuggestionSyntaxItem
        {
            string Name { get; }
            string Description { get; }

            string
        }

        public interface IParserSuggestionSample
        {
            string Parameter { get; }
            string Description { get; }
        }

        private class ParserSuggestion
        {
            public enum Keys : ushort { Size, Created, Modified, Accessed }

            public Keys Key { get; }

            public ParserSuggestion(Keys key) => Key = key;



        }
    }
}
