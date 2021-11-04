using ByteSizeLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface IParser<out T>
    {
        bool CanParse(string item);
        T Parse(string item);
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

    public class TrimParser<T> : IParser<T>
    {
        private readonly IParser<T> parser;

        public TrimParser(IParser<T> parser) => this.parser = parser;

        public bool CanParse(string item) => parser.CanParse(item.Trim());
        public T Parse(string item) => parser.Parse(item.Trim());
    }

    public class DateRangeParser : IParser<DateRange>
    {
        private readonly IParser<DateRange> parser = new TrimParser<DateRange>(new ParserCollection<DateRange>
        {
            new TrimParser<DateRange>(new NamedParser()),
            new TrimParser<DateRange>(new YearParser()),
            new TrimParser<DateRange>(new DayParser()),
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
                return item.Split("..", 2).All(part => string.IsNullOrEmpty(item) || parser.CanParse(part));
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
        private readonly IParser<SizeRange> parser = new TrimParser<SizeRange>(new ParserCollection<SizeRange>
        {
            new TrimParser<SizeRange>(new NamedParser()),
            new TrimParser<SizeRange>(new SizeParser()),
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
                return item.Split("..", 2).All(part => string.IsNullOrEmpty(item) || parser.CanParse(part));
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
