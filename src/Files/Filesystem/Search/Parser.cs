using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface IParser<out T>
    {
        bool CanParse(string input);
        T Parse(string input);
    }

    public interface IRange<out T>
    {
        T MinValue { get; }
        T MaxValue { get; }
    }

    public class ParserCollection<T> : Collection<IParser<T>>, IParser<T>
    {
        public ParserCollection() : base()
        {
        }
        public ParserCollection(IList<IParser<T>> parsers) : base(parsers)
        {
        }

        public bool CanParse(string input) => this.Any(parser => parser.CanParse(input));
        public T Parse(string input) => this.First(parser => parser.CanParse(input)).Parse(input);
    }
}
