﻿using Files.Filesystem;
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

        public ObservableCollection<object> Suggestions { get; } = new ObservableCollection<object>();

        public void ClearSuggestions()
        {
            Suggestions.Clear();
        }

        public void SetSuggestions(IEnumerable<object> suggestions)
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
                var a = new ParserKeySuggestion { Name = "date:", Description = "Date of item" };
                var b = new ParserKeySuggestion { Name = "size:", Description = "Size of item" };

                SetSuggestions(new List<IParserKeySuggestion> { a, b});
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
            public int Compare(object x, object y) => GetKey(y).CompareTo(GetKey(x));
            public new bool Equals(object x, object y) => GetKey(y).Equals(GetKey(x));
            public int GetHashCode(object o) => GetKey(o).GetHashCode();

            private static (ushort, string) GetKey(object o) => o switch
            {
                IParserKeySuggestion suggestion => (1, suggestion.Name),
                ListedItem item => (2, item.ItemPath),
                _ => throw new ArgumentException(),
            };
        }

        private class ParserKeySuggestion : IParserKeySuggestion
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }

    public interface IParserKeySuggestion
    {
        string Name { get; }
        string Description { get; }
    }
}
