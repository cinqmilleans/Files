using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchOption
    {
        ISearchOptionKey Key { get; }
        ISearchOptionValue Value { get; }

        string Text { get; set; }
        string Label { get; }
        bool IsValid { get; }

        string AdvancedQuerySyntax { get; }
    }

    public interface ISearchOptionFactory : IFactory<ISearchOption>
    {
        IReadOnlyDictionary<string, ISearchOptionKey> AllKeys { get; }
        object[] GetSuggestions(string item);
    }

    public interface IFactory<out T>
    {
        bool CanProvide(string item);
        T Provide(string item);
    }

    public class FactoryCollection<T> : Collection<IFactory<T>>, IFactory<T>
    {
        public FactoryCollection() : base()
        {
        }
        public FactoryCollection(IList<IFactory<T>> factories) : base(factories)
        {
        }

        public bool CanProvide(string item)
            => this.Any(provider => provider.CanProvide(item));

        public T Provide(string item)
            => this.First(provider => provider.CanProvide(item)).Provide(item);
    }

    public class SearchOption : ObservableObject, ISearchOption
    {
        public ISearchOptionKey Key { get; }

        private ISearchOptionValue value;
        public ISearchOptionValue Value
        {
            get => value;
            set => Text = Value?.Text ?? string.Empty;
        }

        private string text = string.Empty;
        public string Text
        {
            get => text;
            set
            {
                if (!text.Equals(value ?? string.Empty))
                {
                    text = value ?? string.Empty;
                    isValid = Key.ValueFactory.CanProvide(value);
                    this.value = isValid ? Key.ValueFactory.Provide(value) : null;

                    OnPropertyChanged(nameof(Text));
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(Label));
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string Label => IsValid ? Value.Label : Key.Label;

        private bool isValid = false;
        public bool IsValid => isValid;

        public string AdvancedQuerySyntax => Key.GetAdvancedQuerySyntax(Value);

        public SearchOption(ISearchOptionKey key) : this(key, string.Empty)
        {
        }
        public SearchOption(ISearchOptionKey key, string text) : base()
        {
            Key = key;
            Text = text ?? string.Empty;
        }
    }

    public class SearchOptionFactory : ISearchOptionFactory
    {
        public static SearchOptionFactory Default { get; } = new SearchOptionFactory();

        public IReadOnlyDictionary<string, ISearchOptionKey> AllKeys { get; } = new ISearchOptionKey[]
        {
            new DateSearchOptionKey(),
            new ModifiedSearchOptionKey(),
        }.ToDictionary(key => key.Text);

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

        public object[] GetSuggestions (string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return new object[0];
            }
            if (!item.Contains(':'))
            {
                return AllKeys.Values
                    .Where(key => key.Text.Contains(item))
                    .OrderBy(key => key.Text)
                    .ToArray();
            }

            var (key, value) = Parse(item);
            if (!AllKeys.ContainsKey(key))
            {
                return new object[0];
            }
            var optionKey = AllKeys[key];
            return AllKeys[key].Suggestions
                .Where(suggestion => CanProvide(suggestion))
                .Select(suggestion => Provide(suggestion))
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
