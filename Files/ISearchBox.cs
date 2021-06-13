using Files.Filesystem;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Files
{
    public interface ISearchBox
    {
        event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        event TypedEventHandler<ISearchBox, SearchBoxSuggestionChosenEventArgs> SuggestionChosen;
        event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        event EventHandler<ISearchBox> Escaped;

        void Clear();

        void ClearSuggestions();
        void SetSuggestions(IEnumerable<ListedItem> suggestions);
    }

    public class SearchBoxTextChangedEventArgs
    {
        public string QueryText { get; }

        public SearchBoxTextChangedEventArgs(string queryText) => QueryText = queryText;
    }

    public class SearchBoxSuggestionChosenEventArgs
    {
        public ListedItem SelectedSuggestion { get; }

        public SearchBoxSuggestionChosenEventArgs(ListedItem selectedSuggestion) => SelectedSuggestion = selectedSuggestion;
    }
    public class SearchBoxQuerySubmittedEventArgs
    {
        public string QueryText { get; }
        public ListedItem ChosenSuggestion { get; }

        public SearchBoxQuerySubmittedEventArgs(string queryText)
        {
            QueryText = queryText;
        }
    }
}
