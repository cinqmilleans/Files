using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchOption
    {
        ISearchOptionKey Key { get; }
        ISearchOptionValue Value { get; }
        ushort SortOrder { get; }
    }

    public interface ISearchOptionKey
    {
        string Text { get; }
        string Label { get; }

        ISearchOptionFormat Format { get; }
        string[] SuggestionValues { get; }

        string ProvideFilter(ISearchOptionValue value);
    }

    public interface ISearchOptionValue
    {
        string Text { get; }
        string Label { get; }
    }

    public interface ISearchOptionFormat
    {
        bool CanParseValue(string value);
        ISearchOptionValue ParseValue(string value);
    }

    public interface ISearchOptionKeyProvider
    {
        ISearchOptionKey[] ProvideAllKeys();
    }

    public class SearchOption : ObservableObject, ISearchOption
    {
        private ISearchOptionKey key;
        public ISearchOptionKey Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }

        private ISearchOptionValue value;
        public ISearchOptionValue Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }

        private ushort sortOrder;
        public ushort SortOrder
        {
            get => sortOrder;
            set => SetProperty(ref sortOrder, value);
        }
    }

    public class InvalidSearchOptionValue : ISearchOptionValue
    {
        public string Text { get; }
        public string Label { get; }

        public InvalidSearchOptionValue(string text)
        {
            Text = text;
            Label = "Invalid";
        }
    }

    public class SearchOptionFormatCollection : Collection<ISearchOptionFormat>, ISearchOptionFormat
    {
        public SearchOptionFormatCollection() : base()
        {
        }
        public SearchOptionFormatCollection(IList<ISearchOptionFormat> formats) : base(formats)
        {
        }

        public bool CanParseValue(string value)
            => this.Any(format => format.CanParseValue(value));

        public ISearchOptionValue ParseValue(string value)
            => this.First(format => format.CanParseValue(value)).ParseValue(value);
    }

    public class SearchOptionKeyProvider : ISearchOptionKeyProvider
    {
        public static SearchOptionKeyProvider Default { get; } = new SearchOptionKeyProvider();

        public ISearchOptionKey[] ProvideAllKeys()
        {
            return new ISearchOptionKey[]
            {
                new BeforeSearchOptionKey(),
            }.ToArray();
        }
    }

    public class SearchOptionComparer : IEqualityComparer<ISearchOption>, IComparer<ISearchOption>
    {
        public bool Equals(ISearchOption x, ISearchOption y)
        {
            return x.Key.Text.Equals(y.Key.Text)
                && x.Value.Text.Equals(y.Value.Text);
        }

        public int Compare(ISearchOption x, ISearchOption y)
        {
            if (!Equals(x.SortOrder, y.SortOrder))
            {
                return x.SortOrder.CompareTo(y.SortOrder);
            }
            if (!Equals(x.Key.Text, y.Key.Text))
            {
                return x.Key.Text.CompareTo(y.Key.Text);
            }
            return x.Value.Text.CompareTo(y.Value.Text);
        }

        public int GetHashCode(ISearchOption o)
        {
            return (o.Key.Text, o.Value.Text).GetHashCode();
        }
    }

    public static class SearchOptionExtension
    {
        public static ISearchOption[] ToSuggestions(this ISearchOptionKey key)
            => key.SuggestionValues.Select((value, index) => GetSearchOption(key, value, (ushort)index)).ToArray();

        private static ISearchOption GetSearchOption(ISearchOptionKey key, string valueText, ushort sortOrder)
        {
            var value = key.Format.CanParseValue(valueText)
                ? key.Format.ParseValue(valueText)
                : new InvalidSearchOptionValue(valueText);

            return new SearchOption { Key = key, Value = value, SortOrder = sortOrder };
        }
    }
}
