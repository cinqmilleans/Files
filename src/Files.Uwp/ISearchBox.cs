using Files.Uwp.Filesystem;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Files.Uwp
{
    public interface IStorage
    {
    }

    public interface IDeletableStorage : IStorage
    {
        /// <summary>
        /// Deletes the provided storable item from this folder.
        /// </summary>
        public Task DeleteAsync(bool permanantly = false, CancellationToken cancellationToken = default);
    }

    public interface INamedStorage : IStorage
    {
        string Name { get; }
    }

    public interface IRenamed
        
        
        { string NameA { get; } }
    public interface IB { string NameB { get; } }

    public class C : IA, IB
    {
        public string NameA => "A";
        public string NameB => "B";

        public static string Test(object c)
        {
            if (c is IA and IB m)
            {
                m.
            }

            return string.Empty;
        }
    }



    public interface ISearchBox
    {
        event TypedEventHandler<ISearchBox, SearchBoxTextChangedEventArgs> TextChanged;
        event TypedEventHandler<ISearchBox, SearchBoxQuerySubmittedEventArgs> QuerySubmitted;
        event EventHandler<ISearchBox> Escaped;

        string Query { get; set; }

        void ClearSuggestions();

        void SetSuggestions(IEnumerable<ListedItem> suggestions);
    }

    public class SearchBoxTextChangedEventArgs
    {
        public SearchBoxTextChangeReason Reason { get; }

        public SearchBoxTextChangedEventArgs(SearchBoxTextChangeReason reason) => Reason = reason;

        public SearchBoxTextChangedEventArgs(AutoSuggestionBoxTextChangeReason reason)
        {
            Reason = reason switch
            {
                AutoSuggestionBoxTextChangeReason.UserInput => SearchBoxTextChangeReason.UserInput,
                AutoSuggestionBoxTextChangeReason.SuggestionChosen => SearchBoxTextChangeReason.SuggestionChosen,
                _ => SearchBoxTextChangeReason.ProgrammaticChange
            };
        }
    }

    public class SearchBoxQuerySubmittedEventArgs
    {
        public ListedItem ChosenSuggestion { get; }

        public SearchBoxQuerySubmittedEventArgs(ListedItem chosenSuggestion) => ChosenSuggestion = chosenSuggestion;
    }

    public enum SearchBoxTextChangeReason : ushort
    {
        UserInput,
        ProgrammaticChange,
        SuggestionChosen,
    }
}