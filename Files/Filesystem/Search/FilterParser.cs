using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface IParser<out T>
    {
        bool CanParse(string value);
        T Parse(string value);
    }

    public interface IFilterParser : IParser<ISearchFilter>
    {
        string Name { get; }
        string Description { get; }
        string Syntax { get; }
    }

    public interface IFilterParserFactory
    {
        IEnumerable<string> Names { get; }
        IFilterParser GetParser(string name);
    }

    public class CleanParser<T> : IParser<T>
    {
        private readonly IParser<T> parser;

        public CleanParser(IParser<T> parser) => this.parser = parser;

        public bool CanParse(string item) => parser.CanParse(Clean(item));
        public T Parse(string item) => parser.Parse(Clean(item));

        private static string Clean(string item) => (item ?? string.Empty).Trim();
    }

    public class ParserCollection<T> : Collection<IParser<T>>, IParser<T>
    {
        public ParserCollection() : base()
        {
        }
        public ParserCollection(IList<IParser<T>> parsers) : base(parsers)
        {
        }

        public bool CanParse(string item) => this.Any(parser => parser.CanParse(item));
        public T Parse(string item) => this.First(parser => parser.CanParse(item)).Parse(item);
    }

    public class FilterParserFactory : IFilterParserFactory
    {
        private enum Keys : ushort { SizeRange, Created, Modified, Accessed, Before, After };

        private readonly Lazy<IDictionary<string, Keys>> nameKeys = new Lazy<IDictionary<string, Keys>>(GetNameKeys);
        private IDictionary<string, Keys> NameKeys => nameKeys.Value;

        public IEnumerable<string> Names => NameKeys.Keys.OrderBy(name => name);

        public IFilterParser GetParser(string name) => GetParser(NameKeys[name]);

        private static IDictionary<string, Keys> GetNameKeys()
            => Enum.GetValues(typeof(Keys)).Cast<Keys>().ToDictionary(key => GetParser(key).Name);

        private static IFilterParser GetParser(Keys key) => key switch
        {
            Keys.SizeRange => new SizeRangeParser(),
            Keys.Created => new CreatedParser(),
            Keys.Modified => new ModifiedParser(),
            Keys.Accessed => new AccessedParser(),
            Keys.Before => new BeforeParser(),
            Keys.After => new AfterParser(),
            _ => throw new ArgumentException(),
        };
    }

    public interface IRange<out Item>
    {
        Item MinValue { get; }
        Item MaxValue { get; }
    }

    public class RangeParser<Item> : RangeParser<IRange<Item>, Item>
    {
        public RangeParser(IRange<Item> all, IParser<IRange<Item>> parser) : base(all, parser) {}

        protected override IRange<Item> GetRange(IRange<Item> range) => range;
    }
    public abstract class RangeParser<Range, Item> : IParser<Range> where Range : IRange<Item>
    {
        private Range All { get; }

        private IParser<Range> Parser { get; }

        public RangeParser(Range all, IParser<Range> parser) => (All, Parser) = (all, parser);

        public bool CanParse(string value)
        {
            if (value.StartsWith('<') || value.StartsWith('>'))
            {
                return Parser.CanParse(value.Substring(1));
            }
            if (value.StartsWith(".."))
            {
                return Parser.CanParse(value.Substring(2));
            }
            if (value.EndsWith(".."))
            {
                return Parser.CanParse(value.Substring(0, value.Length - 2));
            }
            if (value.Contains(".."))
            {
                return value.Split("..", 2).All(part => Parser.CanParse(part));
            }
            return Parser.CanParse(value);
        }

        public Range Parse(string item)
        {
            GenericRange range = new(All);

            if (item.StartsWith('<'))
            {
                range.MaxValue = Parser.Parse(item.Substring(1)).MaxValue;
            }
            else if (item.StartsWith('>'))
            {
                range.MinValue = Parser.Parse(item.Substring(1)).MinValue;
            }
            else if (item.StartsWith(".."))
            {
                range.MaxValue = Parser.Parse(item.Substring(2)).MaxValue;
            }
            else if (item.EndsWith(".."))
            {
                range.MinValue = Parser.Parse(item.Substring(0, item.Length - 2)).MinValue;
            }
            else if (item.Contains(".."))
            {
                var parts = item.Split("..", 2);
                range.MinValue = Parser.Parse(parts[0]).MinValue;
                range.MaxValue = Parser.Parse(parts[1]).MaxValue;
            }
            else
            {
                range = new(Parser.Parse(item));
            }
            return GetRange(range);
        }

        protected abstract Range GetRange(IRange<Item> range);

        private class GenericRange : IRange<Item>
        {
            public Item MinValue { get; set; }
            public Item MaxValue { get; set; }

            public GenericRange(IRange<Item> range)
                => (MinValue, MaxValue) = (range.MinValue, range.MaxValue);
        }
    }
}
