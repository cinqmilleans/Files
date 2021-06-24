using System;
using System.Collections.Generic;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface IPeriodSearchOptionValue : ISearchOptionValue
    {
        IPeriod Period { get; }
    }

    public interface IPeriod
    {
        DateTime? MinDate { get; }
        DateTime? MaxDate { get; }

        public string AdvancedQuerySyntax { get; }
    }
    public class Period : IPeriod
    {
        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        private Lazy<string> advancedQuerySyntax;
        public string AdvancedQuerySyntax => advancedQuerySyntax.Value;

        public Period(DateTime? mindate, DateTime? maxDate)
        {
            MinDate = mindate;
            MaxDate = maxDate;

            advancedQuerySyntax = new Lazy<string>(ToAdvancedQuerySyntax);
        }

        private string ToAdvancedQuerySyntax()
        {
            if (!MinDate.HasValue && !MaxDate.HasValue)
            {
                return string.Empty;
            }
            if (!MaxDate.HasValue)
            {
                return $"System.ItemDate:>={MinDate.Value:yyyy-MM-dd}";
            }
            if (!MinDate.HasValue)
            {
                return $"System.ItemDate:<={MaxDate.Value:yyyy-MM-dd}";
            }
            return $"System.ItemDate:{MinDate.Value:yyyy-MM-dd}..{MaxDate.Value:yyyy-MM-dd}";
        }
    }

    public class PeriodSearchOptionValueFactory : IFactory<IPeriodSearchOptionValue>
    {
        private readonly IFactory<IPeriod> factory = PeriodConverter.Default;

        public static PeriodSearchOptionValueFactory Default { get; } = new PeriodSearchOptionValueFactory();

        public bool CanProvide(string text) => factory.CanProvide(text);
        public IPeriodSearchOptionValue Provide(string text) => new PeriodSearchOptionValue(factory.Provide(text));
    }

    public class PeriodSearchOptionValue : IPeriodSearchOptionValue
    {
        public string Text { get; } = string.Empty;
        public string Label { get; } = string.Empty;

        public IPeriod Period { get; set; }

        public PeriodSearchOptionValue(IPeriod period)
        {
            Period = period;

            IReader<IPeriod> reader = PeriodConverter.Default;
            if (reader.CanRead(period))
            {
                Text = reader.ToText(period);
                Label = reader.ToLabel(period);
            }
        }
    }

    public class PeriodConverter : IFactory<IPeriod>, IReader<IPeriod>
    {
        private readonly IFactory<DateTime> minFactory = new FactoryCollection<DateTime> { MomentConverter.Default, MinYearConverter.Default, DateConverter.Default };
        private readonly IFactory<DateTime> maxFactory = new FactoryCollection<DateTime> { MomentConverter.Default, MaxYearConverter.Default, DateConverter.Default };

        private readonly IReader<DateTime> minReader = new ReaderCollection<DateTime> { MomentConverter.Default, MinYearConverter.Default, DateConverter.Default };
        private readonly IReader<DateTime> maxReader = new ReaderCollection<DateTime> { MomentConverter.Default, MaxYearConverter.Default, DateConverter.Default };

        public static PeriodConverter Default { get; } = new PeriodConverter();

        public bool CanProvide(string text)
        {
            if (text.StartsWith(">="))
            {
                return minFactory.CanProvide(text.Substring(2));
            }
            if (text.StartsWith("<=") || text.StartsWith(".."))
            {
                return maxFactory.CanProvide(text.Substring(2));
            }
            if (text.StartsWith(">"))
            {
                return minFactory.CanProvide(text.Substring(1));
            }
            if (text.StartsWith("<"))
            {
                return maxFactory.CanProvide(text.Substring(1));
            }
            if (text.EndsWith(".."))
            {
                return minFactory.CanProvide(text.Substring(0, text.Length - 2));
            }
            if (text.Contains(".."))
            {
                var parts = text.Split("..", 2);
                return minFactory.CanProvide(parts[0]) && maxFactory.CanProvide(parts[1]);
            }
            return minFactory.CanProvide(text) && maxFactory.CanProvide(text);
        }
        public IPeriod Provide(string text)
        {
            if (text.StartsWith(">="))
            {
                return new Period(minFactory.Provide(text.Substring(2)), null);
            }
            if (text.StartsWith("<=") || text.StartsWith(".."))
            {
                return new Period(null, maxFactory.Provide(text.Substring(2)));
            }
            if (text.StartsWith(">"))
            {
                return new Period(minFactory.Provide(text.Substring(1)), null);
            }
            if (text.StartsWith("<"))
            {
                return new Period(null, maxFactory.Provide(text.Substring(1)));
            }
            if (text.EndsWith(".."))
            {
                return new Period(minFactory.Provide(text.Substring(0, text.Length - 2)), null);
            }
            if (text.Contains(".."))
            {
                var parts = text.Split("..", 2);
                return new Period(minFactory.Provide(parts[0]), maxFactory.Provide(parts[1]));
            }
            return new Period(minFactory.Provide(text), maxFactory.Provide(text));
        }

        public bool CanRead(IPeriod period) =>
            (period.MinDate.HasValue && minReader.CanRead(period.MinDate.Value))
        || (period.MaxDate.HasValue && maxReader.CanRead(period.MaxDate.Value));

        public string ToText(IPeriod period)
        {
            if (!period.MaxDate.HasValue)
            {
                return $"<={minReader.ToText(period.MinDate.Value)}";
            }
            if (!period.MinDate.HasValue)
            {
                return $">={maxReader.ToText(period.MaxDate.Value)}";
            }

            string minText = minReader.ToText(period.MinDate.Value);
            string maxText = maxReader.ToText(period.MaxDate.Value);

            if (minText == maxText)
            {
                return minText;
            }

            return $"{minText}..{maxText}";
        }
        public string ToLabel(IPeriod period)
        {
            if (!period.MaxDate.HasValue)
            {
                return $"<= {minReader.ToLabel(period.MinDate.Value)}";
            }
            if (!period.MinDate.HasValue)
            {
                return $">= {maxReader.ToLabel(period.MaxDate.Value)}";
            }

            string minLabel = minReader.ToLabel(period.MinDate.Value);
            string maxLabel = maxReader.ToLabel(period.MaxDate.Value);

            if (minLabel == maxLabel)
            {
                return minLabel;
            }

            return $"{minLabel} -> {maxLabel}";
        }
    }

    public class MomentConverter : IFactory<DateTime>, IReader<DateTime>
    {
        private enum Moments { Today, Yesterday, WeekAgo, MonthAgo, YearAgo }

        private readonly IReadOnlyDictionary<Moments, DateTime> MomentDates;

        public static MomentConverter Default = new MomentConverter();

        public MomentConverter()
        {
            var today = DateTime.Today;
            MomentDates = new Dictionary<Moments, DateTime>
            {
                [Moments.Today] = today,
                [Moments.Yesterday] = today.AddDays(-1),
                [Moments.WeekAgo] = today.AddDays(-7),
                [Moments.MonthAgo] = today.AddMonths(-1),
                [Moments.YearAgo] = today.AddYears(-1),
            };
        }

        public bool CanProvide(string text)
                => Enum.GetNames(typeof(Moments)).Any(moment => moment.ToLower().Equals(text.ToLower()));

        public DateTime Provide(string text) => MomentDates[ToMoment(text)];

        public bool CanRead(DateTime date) => MomentDates.Values.Contains(date);

        public string ToText(DateTime date) => ToMoment(date) switch
        {
            Moments.Today => "today",
            Moments.Yesterday => "yesterday",
            Moments.WeekAgo => "weekAgo",
            Moments.MonthAgo => "monthAgo",
            Moments.YearAgo => "yearAgo",
            _ => throw new ArgumentException()
        };

        public string ToLabel(DateTime date) => ToMoment(date) switch
        {
            Moments.Today => "Today",
            Moments.Yesterday => "Yesterday",
            Moments.WeekAgo => "One week ago",
            Moments.MonthAgo => "One month ago",
            Moments.YearAgo => "One year ago",
            _ => throw new ArgumentException()
        };

        private Moments ToMoment(string text) => Enum.Parse<Moments>(text);
        private Moments ToMoment(DateTime date) => MomentDates.First(i => i.Value == date).Key;
    }

    public class MinYearConverter : IFactory<DateTime>, IReader<DateTime>
    {
        public ushort MinYear { get; } = 1900;
        public ushort MaxYear { get; } = 9999;

        public static MinYearConverter Default { get; } = new MinYearConverter();

        public MinYearConverter()
        {
        }
        public MinYearConverter(ushort minYear, ushort maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;
        }

        public bool CanProvide(string text)
            => ushort.TryParse(text, out ushort year) && year >= MinYear && year <= MaxYear;

        public DateTime Provide(string text) => new DateTime(ushort.Parse(text), 1, 1);

        public bool CanRead(DateTime date) => date.Month == 1 && date.Day == 1;

        public string ToText(DateTime date) => $"{date:yyyy}";
        public string ToLabel(DateTime date) => $"Year {date:yyyy}";
    }
    public class MaxYearConverter : IFactory<DateTime>, IReader<DateTime>
    {
        public ushort MinYear { get; } = 1900;
        public ushort MaxYear { get; } = 9999;

        public static MaxYearConverter Default { get; } = new MaxYearConverter();

        public MaxYearConverter()
        {
        }
        public MaxYearConverter(ushort minYear, ushort maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;
        }

        public bool CanProvide(string text)
            => ushort.TryParse(text, out ushort year) && year >= MinYear && year <= MaxYear;

        public DateTime Provide(string text) => new DateTime(ushort.Parse(text), 12, 31);

        public bool CanRead(DateTime date) => date.Month == 12 && date.Day == 31;

        public string ToText(DateTime date) => $"{date:yyyy}";
        public string ToLabel(DateTime date) => $"Year {date:yyyy}";
    }

    public class DateConverter : IFactory<DateTime>, IReader<DateTime>
    {
        public static DateConverter Default = new DateConverter();

        public bool CanProvide(string text) => DateTime.TryParse(text, out DateTime _);
        public DateTime Provide(string text) => DateTime.Parse(text).Date;

        public bool CanRead(DateTime date) => true;

        public string ToText(DateTime date) => date.ToString("d");
        public string ToLabel(DateTime date) => date.ToString("D");
    }
}
