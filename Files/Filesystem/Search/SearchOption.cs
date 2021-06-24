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

        IFactory<ISearchOptionValue> Format { get; }

        string[] Suggestions { get; }

        string GetAdvancedQuerySyntax(ISearchOptionValue value);
    }

    public interface ISearchOptionValue
    {
        string Text { get; }
        string Label { get; }
    }

    public interface IFactory<out T>
    {
        bool CanProvide(string item);
        T Provide(string item);
    }

    public interface IReader<in T>
    {
        bool CanRead(T value);
        string ToText(T value);
        string ToLabel(T value);
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

    public class ReaderCollection<T> : Collection<IReader<T>>, IReader<T>
    {
        public ReaderCollection() : base()
        {
        }
        public ReaderCollection(IList<IReader<T>> readers) : base(readers)
        {
        }

        public bool CanRead(T value) => this.Any(reader => reader.CanRead(value));
        public string ToText(T value) => this.First(reader => reader.CanRead(value)).ToText(value);
        public string ToLabel(T value) => this.First(reader => reader.CanRead(value)).ToLabel(value);
    }

    public abstract class SearchOptionValue : ISearchOptionValue
    {
        private readonly Lazy<string> text;
        public string Text => text.Value;

        private readonly Lazy<string> label;
        public string Label => label.Value;

        protected SearchOptionValue()
        {
            text = new Lazy<string>(ToText);
            label = new Lazy<string>(ToLabel);
        }

        protected abstract string ToText();
        protected abstract string ToLabel();
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
                    isValid = Key.Format.CanProvide(value);
                    this.value = isValid ? Key.Format.Provide(value) : null;

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
