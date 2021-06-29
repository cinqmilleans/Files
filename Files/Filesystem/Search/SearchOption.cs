using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchOption : INotifyPropertyChanged
    {
        ISearchOptionKey Key { get; }
        ISearchOptionValue Value { get; }

        string Text { get; set; }
        string Label { get; }
        bool IsValid { get; }

        string AdvancedQuerySyntax { get; }
    }

    public interface ISearchOptionKey
    {
        string Text { get; }
        string Label { get; }

        string[] Suggestions { get; }

        ISearchOptionValue GetEmptyValue();

        string GetAdvancedQuerySyntax(ISearchOptionValue value);
    }

    public interface ISearchOptionValue
    {
        string Text { get; set; }
        string Label { get; }
        bool IsValid { get; }
    }

    public interface IFactory<out T>
    {
        bool CanProvide(string item);
        T Provide(string item);
    }
 
    public interface ISearchOptionFactory : IFactory<ISearchOption>
    {
        IReadOnlyDictionary<string, ISearchOptionKey> AllKeys { get; }
        ISearchOptionKey[] GetOptionKeySuggestions(string item);
        ISearchOption[] GetOptionSuggestions(string item);
    }

    public class FactoryCollection<T> : Collection<IFactory<T>>, IFactory<T>
    {
        public FactoryCollection() : base()
        {
        }
        public FactoryCollection(IList<IFactory<T>> factories) : base(factories)
        {
        }

        public bool CanProvide(string item) => this.Any(factory => factory.CanProvide(item));
        public T Provide(string item) => this.First(provider => provider.CanProvide(item)).Provide(item);
    }

    public class SearchOption : ObservableObject, ISearchOption
    {
        public ISearchOptionKey Key { get; }
        public ISearchOptionValue Value { get; }

        public string Text
        {
            get => (Value.IsValid ? Value.Text : Key.Text) ?? string.Empty;
            set
            {
                if (!Text.Equals(value ?? string.Empty))
                {
                    Value.Text = value ?? string.Empty;
                }
            }
        }

        public string Label => IsValid ? Value.Label : Key.Label;

        public bool IsValid => Value.IsValid;

        public string AdvancedQuerySyntax => Key.GetAdvancedQuerySyntax(Value);

        public SearchOption(ISearchOptionKey key) : this(key, string.Empty)
        {
        }
        public SearchOption(ISearchOptionKey key, string text) : base()
        {
            Key = key;
            Value = key.GetEmptyValue();
            Text = text ?? string.Empty;

            if (Value is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += NotifyPropertyChanged_PropertyChanged;
            }
        }

        private void NotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Label));
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public class SearchOptionFactory : ISearchOptionFactory
    {
        private readonly string[] SuggestionTexts;

        public static SearchOptionFactory Default { get; } = new SearchOptionFactory();

        public IReadOnlyDictionary<string, ISearchOptionKey> AllKeys { get; } = new ISearchOptionKey[]
        {
            new DateSearchOptionKey(),
            new ModifiedSearchOptionKey(),
        }.ToDictionary(key => key.Text);

        public SearchOptionFactory()
        {
            SuggestionTexts = AllKeys.Values
                .SelectMany(key => key.Suggestions)
                .Distinct()
                .OrderBy(suggestion => suggestion)
                .ToArray();
        }

        public bool CanProvide(string item)
        {
            var (key, _) = Parse(item);
            return AllKeys.ContainsKey(key);
        }

        public ISearchOption Provide(string item)
        {
            var (key, value) = Parse(item);
            return new SearchOption(AllKeys[key], value);
        }

        public ISearchOptionKey[] GetOptionKeySuggestions (string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return new ISearchOptionKey[0];
            }
            return AllKeys.Values
                .Where(key => key.Text.Contains(item))
                .OrderBy(key => key.Text)
                .ToArray();
        }
        public ISearchOption[] GetOptionSuggestions(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return new ISearchOption[0];
            }
            return SuggestionTexts
                .Where(text => text.Contains(item))
                .OrderBy(text => text)
                .Select(text => Provide(text))
                .ToArray();
        }

        private static (string, string) Parse(string item)
        {
            if (!item.Contains(':'))
            {
                return (item.ToLower(), string.Empty);
            }
            var parts = item.Split(':', 2);
            return (parts[0].ToLower(), parts?[1] ?? string.Empty);
        }
    }
}
