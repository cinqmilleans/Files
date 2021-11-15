using ByteSizeLib;
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

    public interface INamedParser<out T> : IParser<T>, IParserKey
    {
    }

    public interface IMainFilterParser : IParser<ISearchFilter>
    {
        IEnumerable<IParserKey> Keys { get; }
    }

    public interface IParserKey
    {
        string Name { get; }
        string Description { get; }
        string Syntax { get; }
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

    public class MainFilterParser : IMainFilterParser
    {
        private readonly ICollection<INamedParser<ISearchFilter>> Parsers = new Collection<INamedParser<ISearchFilter>>
        {
            new SizeRangeFilterParser(),
            new CreatedFilterParser(),
            new ModifiedFilterParser(),
            new AccessedFilterParser(),
        };

        public IEnumerable<IParserKey> Keys => Parsers;

        public bool CanParse(string value) => Parsers.Any(parser => parser.CanParse(value));
        public ISearchFilter Parse(string value) => Parsers.First(parser => parser.CanParse(value)).Parse(value);
    }

    public abstract class AbstractFilterParser<T> : INamedParser<T> where T : ISearchFilter
    {
        public abstract string Name { get; }
        public virtual string Description => string.Empty;
        public virtual string Syntax => string.Empty;

        public bool CanParse(string value)
        {
            var (name, parameter) = Split(value);
            return Name == name && CanParseParameter(parameter);
        }
        public T Parse(string value)
        {
            var (_, parameter) = Split(value);
            return ParseParameter(parameter);
        }

        protected abstract bool CanParseParameter(string parameter);
        protected abstract T ParseParameter(string parameter);

        private static (string key, string parameter) Split(string value)
        {
            value = (value ?? string.Empty).Trim();

            if (!value.Contains(':'))
            {
                return (value, string.Empty);
            }

            var parts = value.Split(':');
            string name = parts[0].ToLower();
            string parameter = parts[1];

            return (name, parameter);
        }
    }

    public class SizeRangeFilterParser : AbstractFilterParser<ISizeRangeFilter>
    {
        private readonly IParser<SizeRange> parser = new SizeRangeParser();

        public override string Name { get; } = "size";
        public override string Description { get; } = "Size of the item";

        protected override bool CanParseParameter(string parameter) => parser.CanParse(parameter);
        protected override ISizeRangeFilter ParseParameter(string parameter) => new SizeRangeFilter(parser.Parse(parameter));
    }

    public class CreatedFilterParser : AbstractFilterParser<CreatedFilter>
    {
        private readonly IParser<DateRange> parser = new DateRangeParser();

        public override string Name { get; } = "created";
        public override string Description { get; } = "Date of creation";
        public override string Syntax { get; } =
            "Valid values are date ranges such as:11/05/04[Items with a date before 11/05/04]\nAs well as relative values such as:yesterday:lastweek";

        protected override bool CanParseParameter(string parameter) => parser.CanParse(parameter);
        protected override CreatedFilter ParseParameter(string parameter) => new CreatedFilter(parser.Parse(parameter));
    }
    public class ModifiedFilterParser : AbstractFilterParser<ModifiedFilter>
    {
        private readonly IParser<DateRange> parser = new DateRangeParser();

        public override string Name { get; } = "modified";
        public override string Description { get; } = "Size of the last modification";
        public override string Syntax { get; } =
            "Valid values are date ranges such as:11/05/04[Items with a date before 11/05/04]\nAs well as relative values such as:yesterday:lastweek";

        protected override bool CanParseParameter(string parameter) => parser.CanParse(parameter);
        protected override ModifiedFilter ParseParameter(string parameter) => new ModifiedFilter(parser.Parse(parameter));
    }
    public class AccessedFilterParser : AbstractFilterParser<AccessedFilter>
    {
        private readonly IParser<DateRange> parser = new DateRangeParser();

        public override string Name { get; } = "accessed";
        public override string Description { get; } = "Size of the last access";

        protected override bool CanParseParameter(string parameter) => parser.CanParse(parameter);
        protected override AccessedFilter ParseParameter(string parameter) => new AccessedFilter(parser.Parse(parameter));
    }

    public class DateRangeParser : IParser<DateRange>
    {
        private readonly IParser<DateRange> parser =
            new CleanParser<DateRange>(new ParserCollection<DateRange>
            {
                new CleanParser<DateRange>(new NamedParser()),
                new CleanParser<DateRange>(new YearParser()),
                new CleanParser<DateRange>(new DayParser()),
            });

        public bool CanParse(string item)
        {
            if (item.StartsWith('<') || item.StartsWith('>'))
            {
                return parser.CanParse(item.Substring(1));
            }
            if (item.StartsWith(".."))
            {
                return parser.CanParse(item.Substring(2));
            }
            if (item.EndsWith(".."))
            {
                return parser.CanParse(item.Substring(0, item.Length - 2));
            }
            if (item.Contains(".."))
            {
                return item.Split("..", 2).All(part => parser.CanParse(part));
            }
            return parser.CanParse(item);
        }

        public DateRange Parse(string item)
        {
            Date minDate = Date.MinValue;
            Date maxDate = Date.Today;

            if (item.StartsWith('<'))
            {
                maxDate = parser.Parse(item.Substring(1)).MaxDate;
            }
            else if (item.StartsWith('>'))
            {
                minDate = parser.Parse(item.Substring(1)).MinDate;
            }
            else if (item.StartsWith(".."))
            {
                maxDate = parser.Parse(item.Substring(2)).MaxDate;
            }
            else if (item.EndsWith(".."))
            {
                minDate = parser.Parse(item.Substring(0, item.Length - 2)).MinDate;
            }
            else if (item.Contains(".."))
            {
                var parts = item.Split("..", 2);
                minDate = parser.Parse(parts[0]).MinDate;
                maxDate = parser.Parse(parts[1]).MaxDate;
            }
            else
            {
                (minDate, maxDate) = parser.Parse(item);
            }
            return new DateRange(minDate, maxDate);
        }

        private class NamedParser : IParser<DateRange>
        {
            public IDictionary<string, DateRange> Nameds = new List<DateRange>
            {
                DateRange.Today,
                DateRange.Yesterday,
                DateRange.ThisWeek,
                DateRange.LastWeek,
                DateRange.ThisMonth,
                DateRange.LastMonth,
                DateRange.ThisYear,
                DateRange.Older,
            }.ToDictionary(range => range.ToString("n").ToLower());

            public bool CanParse(string item) => Nameds.ContainsKey(item.ToLower());
            public DateRange Parse(string item) => Nameds[item.ToLower()];
        }

        private class YearParser : IParser<DateRange>
        {
            private const ushort minYear = 1900;
            private const ushort maxYear = 2299;

            public bool CanParse(string item)
                => ushort.TryParse(item, out ushort year) && year >= minYear && year <= maxYear;

            public DateRange Parse(string item)
            {
                ushort year = ushort.Parse(item);
                Date minDate = new Date(year, 1, 1);
                Date maxDate = new Date(year, 12, 31);
                return new DateRange(minDate, maxDate);
            }
        }

        private class DayParser : IParser<DateRange>
        {
            private readonly DateTime minDay = new DateTime(1900, 1, 1);
            private readonly DateTime maxDay = new DateTime(2299, 12, 31);

            public bool CanParse(string item)
                => DateTime.TryParse(item, out DateTime day) && day >= minDay && day <= maxDay;

            public DateRange Parse(string item)
            {
                var date = new Date(DateTime.Parse(item));
                return new DateRange(date, date);
            }
        }
    }

    public class SizeRangeParser : IParser<SizeRange>
    {
        private readonly IParser<SizeRange> parser =
            new CleanParser<SizeRange>(new ParserCollection<SizeRange>
            {
                new CleanParser<SizeRange>(new NamedParser()),
                new CleanParser<SizeRange>(new SizeParser()),
            });

        public bool CanParse(string item)
        {
            if (item.StartsWith('<') || item.StartsWith('>'))
            {
                return parser.CanParse(item.Substring(1));
            }
            if (item.StartsWith(".."))
            {
                return parser.CanParse(item.Substring(2));
            }
            if (item.EndsWith(".."))
            {
                return parser.CanParse(item.Substring(0, item.Length - 2));
            }
            if (item.Contains(".."))
            {
                return item.Split("..", 2).All(part => parser.CanParse(part));
            }
            return parser.CanParse(item);
        }

        public SizeRange Parse(string item)
        {
            Size minSize = Size.MinValue;
            Size maxSize = Size.MaxValue;

            if (item.StartsWith('<'))
            {
                maxSize = parser.Parse(item.Substring(1)).MaxSize;
            }
            else if (item.StartsWith('>'))
            {
                minSize = parser.Parse(item.Substring(1)).MinSize;
            }
            else if (item.StartsWith(".."))
            {
                maxSize = parser.Parse(item.Substring(2)).MaxSize;
            }
            else if (item.EndsWith(".."))
            {
                minSize = parser.Parse(item.Substring(0, item.Length - 2)).MinSize;
            }
            else if (item.Contains(".."))
            {
                var parts = item.Split("..", 2);
                minSize = parser.Parse(parts[0]).MinSize;
                maxSize = parser.Parse(parts[1]).MaxSize;
            }
            else
            {
                (minSize, maxSize) = parser.Parse(item);
            }
            return new SizeRange(minSize, maxSize);
        }

        private class NamedParser : IParser<SizeRange>
        {
            public IDictionary<string, SizeRange> Nameds = new List<SizeRange>
            {
                SizeRange.Empty,
                SizeRange.Tiny,
                SizeRange.Small,
                SizeRange.Medium,
                SizeRange.Large,
                SizeRange.VeryLarge,
                SizeRange.Huge,
            }.ToDictionary(range => range.ToString("n").ToLower());

            public bool CanParse(string item) => Nameds.ContainsKey(item.ToLower());
            public SizeRange Parse(string item) => Nameds[item.ToLower()];
        }

        private class SizeParser : IParser<SizeRange>
        {
            public bool CanParse(string item)
                => ByteSize.TryParse(item, out ByteSize _);

            public SizeRange Parse(string item)
            {
                var size = ByteSize.Parse(item);
                return new SizeRange(size, size);
            }
        }
    }
}
