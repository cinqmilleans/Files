using Files.Filesystem;
using Files.Filesystem.Search;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Files.ViewModels.Search
{
    public interface ISearchSuggestion
    {
        object Data { get; }
        string TextMemberPath { get; }
    }

    public interface ISearchSuggestionCollection : IEnumerable<ISearchSuggestion>, INotifyCollectionChanged
    {
        void Clear<T>();
        void Set<T>(IEnumerable<ISearchSuggestion> suggestions);
    }

    public abstract class SearchSuggestion<T> : ISearchSuggestion
    {
        object ISearchSuggestion.Data => Data;

        public T Data { get; }
        public string TextMemberPath { get; }

        public SearchSuggestion(T data, string textMemberPath)
        {
            Data = data;
            TextMemberPath = textMemberPath;
        }
    }

    public class OptionKeySearchSuggestion : SearchSuggestion<ISearchOptionKey>
    {
        public OptionKeySearchSuggestion(ISearchOptionKey data, string textMemberPath) : base(data, textMemberPath)
        {
        }
    }

    public class OptionSearchSuggestion : SearchSuggestion<ISearchOption>
    {
        public OptionSearchSuggestion(ISearchOption data, string textMemberPath) : base(data, textMemberPath)
        {
        }
    }

    public class ItemSearchSuggestion : SearchSuggestion<ListedItem>
    {
        public ItemSearchSuggestion(ListedItem data, string textMemberPath) : base(data, textMemberPath)
        {
        }
    }

    public class SearchSuggestionCollection : ObservableCollection<ISearchSuggestion>, ISearchSuggestionCollection
    {
        private readonly SuggestionComparer comparer = new SuggestionComparer();

        public void Clear<T>()
        {
            var oldSuggestions = this.Where(suggestion => suggestion.Data is T).ToList();
            foreach (var oldSuggestion in oldSuggestions)
            {
                Remove(oldSuggestion);
            }
        }
        public void Set<T>(IEnumerable<ISearchSuggestion> suggestions)
        {
            var insertSuggestions = suggestions.OrderBy(suggestion => suggestion, comparer).ToList();
            var currentSuggestions = this.Where(suggestion => suggestion.Data is T).ToList();

            var oldSuggestions = currentSuggestions.Except(insertSuggestions, comparer).ToList();
            foreach (var oldSuggestion in oldSuggestions)
            {
                Remove(oldSuggestion);
            }

            var newSuggestions = insertSuggestions.Except(currentSuggestions, comparer).ToList();
            foreach (var newSuggestion in newSuggestions)
            {
                var indexSuggestion = this.FirstOrDefault(suggestion => comparer.Compare(suggestion, newSuggestion) > 1);
                if (!(indexSuggestion is null))
                {
                    Insert(IndexOf(indexSuggestion), newSuggestion);
                }
                else
                {
                    Add(newSuggestion);
                }
            }
        }
    }

    internal class SuggestionComparer : IEqualityComparer<ISearchSuggestion>, IComparer<ISearchSuggestion>
    {
        private readonly SearchOptionComparer optionComparer = new SearchOptionComparer();

        public bool Equals(ISearchSuggestion x, ISearchSuggestion y)
        {
            if (x.Data is ISearchOptionKey optionKeyX && y.Data is ISearchOptionKey optionKeyY)
            {
                return optionKeyX.Text.Equals(optionKeyY.Text);
            }
            if (x.Data is ISearchOption optionX && y.Data is ISearchOption optionY)
            {
                return optionComparer.Equals(optionX, optionY);
            }
            if (x.Data is ListedItem itemX && y.Data is ListedItem itemY)
            {
                return itemX.ItemPath.Equals(itemY.ItemPath);
            }
            throw new ArgumentException();
        }

        public int Compare(ISearchSuggestion x, ISearchSuggestion y)
        {
            if (x.Data is ISearchOptionKey optionKeyX && y.Data is ISearchOptionKey optionKeyY)
            {
                return optionKeyX.Text.CompareTo(optionKeyY.Text);
            }
            if (x.Data is ISearchOption optionX && y.Data is ISearchOption optionY)
            {
                return optionComparer.Compare(optionX, optionY);
            }
            if (x.Data is ListedItem itemX && y.Data is ListedItem itemY)
            {
                return itemX.ItemPath.CompareTo(itemY.ItemPath);
            }
            return GetTypeOrder(x.Data).CompareTo(GetTypeOrder(y.Data));
        }

        public int GetHashCode(ISearchSuggestion o)
        {
            if (o.Data is ISearchOptionKey optionKey)
            {
                return optionKey.Text.GetHashCode();
            }
            if (o.Data is ISearchOption option)
            {
                return optionComparer.GetHashCode(option);
            }
            if (o.Data is ListedItem item)
            {
                return item.ItemPath.GetHashCode();
            }
            throw new ArgumentException();
        }

        private static ushort GetTypeOrder(object data)
        {
            if (data is ISearchOptionKey)
            {
                return 1;
            }
            if (data is ISearchOption)
            {
                return 2;
            }
            if (data is ListedItem)
            {
                return 3;
            }
            throw new ArgumentException();
        }
    }
}
