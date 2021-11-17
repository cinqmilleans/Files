using Files.Extensions;
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
        IEnumerable<string> Alias { get; }

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
        private enum Keys : ushort { SizeRange, Created, Modified, Accessed };

        private readonly Lazy<IDictionary<string, Keys>> nameKeys = new Lazy<IDictionary<string, Keys>>(GetNameKeys);
        private IDictionary<string, Keys> NameKeys => nameKeys.Value;

        public IEnumerable<string> Names => NameKeys.Keys.OrderBy(name => name);

        public IFilterParser GetParser(string name) => GetParser(NameKeys[name]);

        private static IDictionary<string, Keys> GetNameKeys()
        {
            var nameKeys = new Dictionary<string, Keys>();

            var keys = Enum.GetValues(typeof(Keys)).Cast<Keys>();
            foreach (var key in keys)
            {
                var parser = GetParser(key);
                parser.Alias.Append(parser.Name).ForEach(name => nameKeys.Add(name, key));
            }

            return nameKeys;
        }

        private static IFilterParser GetParser(Keys key) => key switch
        {
            Keys.SizeRange => new SizeRangeParser(),
            Keys.Created => new CreatedParser(),
            Keys.Modified => new ModifiedParser(),
            Keys.Accessed => new AccessedParser(),
            _ => throw new ArgumentException(),
        };
    }
}
