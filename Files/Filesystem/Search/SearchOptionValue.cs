using System;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISearchOptionValue
    {
        string Text { get; }
        string Label { get; }
    }

    public interface IAdvancedQuerySyntaxValue : ISearchOptionValue
    {
        string AdvancedQuerySyntax { get; }
    }

    public interface IPeriodSearchOptionValue : IAdvancedQuerySyntaxValue
    {
        IPeriodSearchOptionValue MinPeriod { get; }
        IPeriodSearchOptionValue MaxPeriod { get; }
    }

    public abstract class PeriodSearchOptionValue : IPeriodSearchOptionValue
    {
        private readonly Lazy<string> text;
        public string Text => text.Value;
        protected abstract string GetText();

        private readonly Lazy<string> label;
        public string Label => label.Value;
        protected virtual string GetLabel() => GetText();

        private readonly Lazy<IPeriodSearchOptionValue> minPeriod;
        public IPeriodSearchOptionValue MinPeriod => minPeriod.Value;
        protected virtual IPeriodSearchOptionValue GetMinPeriod() => this;

        private readonly Lazy<IPeriodSearchOptionValue> maxPeriod;
        public IPeriodSearchOptionValue MaxPeriod => maxPeriod.Value;
        protected virtual IPeriodSearchOptionValue GetMaxPeriod() => this;

        private readonly Lazy<string> advancedQuerySyntax;
        public string AdvancedQuerySyntax => advancedQuerySyntax.Value;
        protected virtual string GetAdvancedQuerySyntax() => GetText();

        public PeriodSearchOptionValue()
        {
            text = new Lazy<string>(GetText);
            label = new Lazy<string>(GetLabel);
            minPeriod = new Lazy<IPeriodSearchOptionValue>(GetMinPeriod);
            maxPeriod = new Lazy<IPeriodSearchOptionValue>(GetMaxPeriod);
            advancedQuerySyntax = new Lazy<string>(GetAdvancedQuerySyntax);
        }
    }

    public class IntervalPeriodSearchOptionValueFactory : IFactory<IntervalPeriodSearchOptionValue>
    {
        public static IntervalPeriodSearchOptionValueFactory Default { get; } = new IntervalPeriodSearchOptionValueFactory();

        private readonly IFactory<IPeriodSearchOptionValue> factory = new FactoryCollection<IPeriodSearchOptionValue>
        {
            MomentPeriodSearchOptionValueFactory.Default,
            YearPeriodSearchOptionValueFactory.Default,
            DatePeriodSearchOptionValueFactory.Default,
        };

        public bool CanProvide(string item)
        {
            if (item.StartsWith(">=") || item.StartsWith("<=") || item.StartsWith(".."))
            {
                return factory.CanProvide(item.Substring(2));
            }
            if (item.StartsWith(">") || item.StartsWith("<"))
            {
                return factory.CanProvide(item.Substring(2));
            }
            if (item.EndsWith(".."))
            {
                return factory.CanProvide(item.Substring(0, item.Length - 2));
            }
            if (item.Contains(".."))
            {
                var parts = item.Split("..", 2);
                return factory.CanProvide(parts[0]) && factory.CanProvide(parts[1]);
            }
            return factory.CanProvide(item);
        }

        public IntervalPeriodSearchOptionValue Provide(string item)
        {
            if (item.StartsWith(">="))
            {
                return new IntervalPeriodSearchOptionValue(factory.Provide(item.Substring(2)), null);
            }
            if (item.StartsWith(">="))
            {
                return new IntervalPeriodSearchOptionValue(null, factory.Provide(item.Substring(2)));
            }
            if (item.StartsWith(">"))
            {
                return new IntervalPeriodSearchOptionValue(factory.Provide(item.Substring(1)), null);
            }
            if (item.StartsWith(">"))
            {
                return new IntervalPeriodSearchOptionValue(null, factory.Provide(item.Substring(1)));
            }
            if (item.StartsWith(".."))
            {
                return new IntervalPeriodSearchOptionValue(null, factory.Provide(item.Substring(2)));
            }
            if (item.EndsWith(".."))
            {
                return new IntervalPeriodSearchOptionValue(factory.Provide(item.Substring(0, item.Length - 2)), null);
            }
            if (item.Contains(".."))
            {
                var parts = item.Split("..", 2);
                return new IntervalPeriodSearchOptionValue(factory.Provide(parts[0]), factory.Provide(parts[1]));
            }
            return new IntervalPeriodSearchOptionValue(factory.Provide(item));
        }
    }
    public class IntervalPeriodSearchOptionValue : IPeriodSearchOptionValue
    {
        public enum Directions { None, Before, After, Interval }
        public Directions Direction { get; }

        public string Text => Direction switch
        {
            Directions.Before => $"<={MaxPeriod.Text}",
            Directions.After => $"<={MinPeriod.Text}",
            Directions.Interval => $"{MinPeriod.Text}..{MaxPeriod.Text}",
            _ => string.Empty
        };
        public string Label => Direction switch
        {
            Directions.Before => $"<= {MaxPeriod.Label}",
            Directions.After => $"<= {MinPeriod.Label}",
            Directions.Interval => $"{MinPeriod.Label} -> {MaxPeriod.Label}",
            _ => string.Empty
        };
        public string AdvancedQuerySyntax => Direction switch
        {
            Directions.Before => $"<={MaxPeriod.AdvancedQuerySyntax}",
            Directions.After => $"<={MinPeriod.AdvancedQuerySyntax}",
            Directions.Interval => $"{MinPeriod.AdvancedQuerySyntax}..{MaxPeriod.AdvancedQuerySyntax}",
            _ => string.Empty
        };

        public IPeriodSearchOptionValue MinPeriod { get; }
        public IPeriodSearchOptionValue MaxPeriod { get; }

        public bool HasMinPeriod => !(MinPeriod is null);
        public bool HasMaxPeriod => !(MaxPeriod is null);

        public IntervalPeriodSearchOptionValue(IPeriodSearchOptionValue period) : this(period, period)
        {
        }
        public IntervalPeriodSearchOptionValue(IPeriodSearchOptionValue minPeriod, IPeriodSearchOptionValue maxPeriod)
        {
            MinPeriod = minPeriod;
            MaxPeriod = maxPeriod;
            Direction = GetDirection();
        }

        private Directions GetDirection()
        {
            if (HasMinPeriod && HasMaxPeriod)
            {
                return Directions.Interval;
            }
            if (HasMaxPeriod)
            {
                return Directions.Before;
            }
            if (HasMinPeriod)
            {
                return Directions.After;
            }
            return Directions.None;
        }
    }

    public class MomentPeriodSearchOptionValueFactory : IFactory<MomentPeriodSearchOptionValue>
    {
        public static MomentPeriodSearchOptionValueFactory Default { get; } = new MomentPeriodSearchOptionValueFactory();

        public bool CanProvide(string item)
            => Enum
                .GetNames(typeof(MomentPeriodSearchOptionValue.Moments))
                .Select(moment => moment.ToLower())
                .Contains(item.ToLower());

        public MomentPeriodSearchOptionValue Provide(string item)
            => new MomentPeriodSearchOptionValue(Enum.Parse<MomentPeriodSearchOptionValue.Moments>(item, true));
    }
    public class MomentPeriodSearchOptionValue : PeriodSearchOptionValue
    {
        public enum Moments { Today, Yesterday, WeekAgo, MonthAgo, YearAgo }

        public Moments Moment { get; }

        public MomentPeriodSearchOptionValue(Moments moment) => Moment = moment;

        protected override string GetText() => Moment switch
        {
            Moments.Today => "today",
            Moments.Yesterday => "yesterday",
            Moments.WeekAgo => "weekAgo",
            Moments.MonthAgo => "monthAgo",
            Moments.YearAgo => "yearAgo",
            _ => throw new ArgumentException()
        };
        protected override string GetLabel() => Moment switch
        {
            Moments.Today => "Today",
            Moments.Yesterday => "Yesterday",
            Moments.WeekAgo => "A week ago",
            Moments.MonthAgo => "A month ago",
            Moments.YearAgo => "A year ago",
            _ => throw new ArgumentException()
        };

        protected override IPeriodSearchOptionValue GetMinPeriod() => new DatePeriodSearchOptionValue(GetDate());
        protected override IPeriodSearchOptionValue GetMaxPeriod() => new DatePeriodSearchOptionValue(DateTime.Today);

        private DateTime GetDate() => Moment switch
        {
            Moments.Today => DateTime.Today,
            Moments.Yesterday => DateTime.Today.AddDays(-1),
            Moments.WeekAgo => DateTime.Today.AddDays(-7),
            Moments.MonthAgo => DateTime.Today.AddMonths(-1),
            Moments.YearAgo => DateTime.Today.AddYears(-1),
            _ => throw new ArgumentException()
        };
    }

    public class YearPeriodSearchOptionValueFactory : IFactory<YearPeriodSearchOptionValue>
    {
        public static YearPeriodSearchOptionValueFactory Default { get; } = new YearPeriodSearchOptionValueFactory();

        public ushort MinYear { get; } = 1900;
        public ushort MaxYear { get; } = 9999;

        public YearPeriodSearchOptionValueFactory()
        {
        }
        public YearPeriodSearchOptionValueFactory(ushort minYear, ushort maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;
        }

        public bool CanProvide(string item)
            => ushort.TryParse(item, out ushort year) && year >= MinYear && year <= MaxYear;

        public YearPeriodSearchOptionValue Provide(string item)
            => new YearPeriodSearchOptionValue(ushort.Parse(item));
    }
    public class YearPeriodSearchOptionValue : PeriodSearchOptionValue
    {
        public ushort Year { get; }

        public YearPeriodSearchOptionValue(ushort year) => Year = year;

        protected override string GetText() => $"{Year}";
        protected override string GetLabel() => $"Year {Year}";

        protected override IPeriodSearchOptionValue GetMinPeriod() => new DatePeriodSearchOptionValue(new DateTime(Year, 1, 1));
        protected override IPeriodSearchOptionValue GetMaxPeriod() => new DatePeriodSearchOptionValue(new DateTime(Year, 12, 31));
    }

    public class DatePeriodSearchOptionValueFactory : IFactory<DatePeriodSearchOptionValue>
    {
        public static DatePeriodSearchOptionValueFactory Default { get; } = new DatePeriodSearchOptionValueFactory();

        public bool CanProvide(string item) => DateTime.TryParse(item, out DateTime _);

        public DatePeriodSearchOptionValue Provide(string item) => new DatePeriodSearchOptionValue(DateTime.Parse(item));
    }
    public class DatePeriodSearchOptionValue : PeriodSearchOptionValue
    {
        public DateTime Date { get; }

        public DatePeriodSearchOptionValue(DateTime date) => Date = date.Date;

        protected override string GetText() => $"{Date:d}";
        protected override string GetLabel() => $"{Date:D}";

        protected override string GetAdvancedQuerySyntax() => $"{Date:yyyy-MM-dd}";
    }
}
