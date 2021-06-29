using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    #region contract
    public interface IPeriodSearchOptionValue : ISearchOptionValue, IPeriod, INotifyPropertyChanged
    {
        public new DateTime? MinDate { get; set; }
        public new DateTime? MaxDate { get; set; }
    }

    public interface IPeriodFactory : IFactory<IPeriod>
    {
        IPeriod ToPeriod(DateTime? minDate, DateTime? maxDate);
    }

    public interface IPeriod
    {
        string Text { get; }
        string Label { get; }

        DateTime? MinDate { get; }
        DateTime? MaxDate { get; }
    }
    #endregion

    #region option
    public class DateSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "date";
        public virtual string Label => "Date of creation";

        public string[] Suggestions { get; } = new string[] { "date:today", "date:yesterday", "date:weekago", "date:monthago", "date:yearago" };

        public ISearchOptionValue GetEmptyValue() => new PeriodSearchOptionValue();

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IPeriod period)
            {
                return $"System.ItemDate:{period.ToAdvancedQuerySyntax()}";
            }
            return string.Empty;
        }
    }

    public class ModifiedSearchOptionKey : ISearchOptionKey
    {
        public virtual string Text => "modified";
        public virtual string Label => "Date of last modification";

        public ISearchOptionValue GetEmptyValue() => new PeriodSearchOptionValue();

        public string[] Suggestions { get; } = new string[0];

        public string GetAdvancedQuerySyntax(ISearchOptionValue value)
        {
            if (value is IPeriod period)
            {
                return $"System.DateModified:{period.ToAdvancedQuerySyntax()}";
            }
            return string.Empty;
        }
    }

    public class PeriodSearchOptionValue : ObservableObject, IPeriodSearchOptionValue
    {
        private readonly IPeriodFactory factory = PeriodFactory.Default;

        private IPeriod period = AlwaysPeriod.Default;

        private string text = string.Empty;
        public string Text
        {
            get => text;
            set
            {
                if (SetProperty(ref text, value ?? string.Empty))
                {
                    period = factory.CanProvide(value) ? factory.Provide(value) : AlwaysPeriod.Default;
                    OnPropertyChanged(nameof(Label));
                    OnPropertyChanged(nameof(IsValid));
                    OnPropertyChanged(nameof(MinDate));
                    OnPropertyChanged(nameof(MaxDate));
                }
            }
        }

        public string Label => IsValid ? period.Label : string.Empty;

        public bool IsValid => period.MinDate.HasValue || period.MaxDate.HasValue;

        public DateTime? MinDate
        {
            get => period.MinDate;
            set => Text = factory.ToPeriod(value, period.MaxDate).Text;
        }
        public DateTime? MaxDate
        {
            get => period.MaxDate;
            set => Text = factory.ToPeriod(period.MinDate, value).Text;
        }
    }
    #endregion

    #region service
    public static class PeriodExtension
    {
        public static string ToAdvancedQuerySyntax(this IPeriod period)
        {
            if (!period.MinDate.HasValue && !period.MaxDate.HasValue)
            {
                return string.Empty;
            }
            if (!period.MaxDate.HasValue)
            {
                return $">{period.MinDate:yyyy-MM-dd}";
            }
            if (!period.MinDate.HasValue)
            {
                return $"<{period.MaxDate:yyyy-MM-dd}";
            }
            return $"{period.MinDate:yyyy-MM-dd}..{period.MaxDate:yyyy-MM-dd}";
        }
    }

    public class PeriodFactory : IPeriodFactory
    {
        private readonly IPeriod[] momentPeriods = MomentPeriodFactory.Default.Periods;
        private readonly YearPeriodFactory yearFactory = YearPeriodFactory.Default;

        private readonly IFactory<IPeriod> factory = new FactoryCollection<IPeriod>
        {
            MomentPeriodFactory.Default,
            YearPeriodFactory.Default,
            DatePeriodFactory.Default,
        };

        public static PeriodFactory Default = new PeriodFactory();

        public bool CanProvide(string text)
        {
            text = (text ?? string.Empty).ToLower();
            if (text.StartsWith('<') || text.StartsWith('>'))
            {
                return factory.CanProvide(text.Substring(1));
            }
            if (text.Contains(".."))
            {
                var (minText, maxText) = GetInterval(text);
                return factory.CanProvide(minText) && factory.CanProvide(maxText);
            }
            return factory.CanProvide(text);
        }
        public IPeriod Provide(string text)
        {
            text = (text ?? string.Empty).ToLower();
            if (text.StartsWith('<'))
            {
                return new BeforePeriod(factory.Provide(text.Substring(1)));
            }
            if (text.StartsWith('>'))
            {
                return new AfterPeriod(factory.Provide(text.Substring(1)));
            }
            if (text.EndsWith(".."))
            {
                return new BeforePeriod(factory.Provide(text.Substring(0, text.Length - 2)));
            }
            if (text.StartsWith(".."))
            {
                return new AfterPeriod(factory.Provide(text.Substring(2)));
            }
            if (text.Contains(".."))
            {
                var (minText, maxText) = GetInterval(text);
                return new IntervalPeriod(factory.Provide(minText), factory.Provide(maxText));
            }
            return factory.Provide(text);
        }

        public IPeriod ToPeriod(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue && !maxDate.HasValue)
            {
                return AlwaysPeriod.Default;
            }
            if (!minDate.HasValue)
            {
                return new BeforePeriod(ToMaxPeriod(maxDate.Value));
            }
            if (!maxDate.HasValue)
            {
                return new AfterPeriod(ToMinPeriod(minDate.Value));
            }

            foreach (var moment in momentPeriods)
            {
                if (moment.MinDate == minDate && moment.MaxDate == maxDate)
                {
                    return moment;
                }
            }

            if (minDate.Value == maxDate.Value)
            {
                return new DatePeriod(minDate.Value);
            }

            bool isMinYear = yearFactory.IsMinDate(minDate.Value);
            bool isMaxYear = yearFactory.IsMaxDate(maxDate.Value);

            if (isMinYear && isMaxYear && minDate.Value.Year == maxDate.Value.Year)
            {
                return new YearPeriod((ushort)minDate.Value.Year);
            }

            return new IntervalPeriod(ToMinPeriod(minDate.Value), ToMaxPeriod(maxDate.Value));

            IPeriod ToMinPeriod(DateTime date)
            {
                foreach (var moment in momentPeriods)
                {
                    if (moment.MinDate == date)
                    {
                        return moment;
                    }
                }
                if (yearFactory.IsMinDate(date))
                {
                    return new YearPeriod((ushort)date.Year);
                }
                return new DatePeriod(date);
            }
            IPeriod ToMaxPeriod(DateTime date)
            {
                foreach (var moment in momentPeriods)
                {
                    if (moment.MaxDate == date)
                    {
                        return moment;
                    }
                }
                if (yearFactory.IsMaxDate(date))
                {
                    return new YearPeriod((ushort)date.Year);
                }
                return new DatePeriod(date);
            }
        }

        private static (string minText, string maxText) GetInterval(string text)
        {
            var parts = text.Split("..");
            return (parts[0], parts[1]);
        }
    }

    public class DatePeriodFactory : IFactory<DatePeriod>
    {
        public static DatePeriodFactory Default = new DatePeriodFactory();

        public bool CanProvide(string text) => DateTime.TryParse(text, out DateTime _);
        public DatePeriod Provide(string text) => new DatePeriod(DateTime.Parse(text));
    }

    public class YearPeriodFactory : IFactory<YearPeriod>
    {
        public static YearPeriodFactory Default = new YearPeriodFactory();

        public ushort MinYear { get; } = 1900;
        public ushort MaxYear { get; } = 9999;

        public YearPeriodFactory()
        {
        }
        public YearPeriodFactory(ushort minYear, ushort maxYear)
        {
            MinYear = minYear;
            MaxYear = maxYear;
        }

        public bool CanProvide(string text) => ushort.TryParse(text, out ushort year) && IsValid(year);
        public YearPeriod Provide(string text) => new YearPeriod(ushort.Parse(text));

        public bool IsMinDate(DateTime date) => date.Month == 1 && date.Day == 1 && IsValid(date.Year);
        public bool IsMaxDate(DateTime date) => date.Month == 12 && date.Day == 31 && IsValid(date.Year);

        private bool IsValid(int year) => year >= MinYear && year <= MaxYear;
    }

    public class MomentPeriodFactory : IFactory<IPeriod>
    {
        private readonly IDictionary<string, IPeriod> moments;

        public static MomentPeriodFactory Default = new MomentPeriodFactory();

        public MomentPeriodFactory() : this(DateTime.Today)
        {
        }
        public MomentPeriodFactory(DateTime today)
        {
            moments = new Dictionary<string, IPeriod>
            {
                ["today"] = new TodayPeriod(today),
                ["yesterday"] = new YesterdayPeriod(today),
                ["thisweek"] = new ThisWeekPeriod(today),
                ["lastweek"] = new LastWeekPeriod(today),
                ["thismonth"] = new ThisMonthPeriod(today),
                ["lastmonth"] = new LastMonthPeriod(today),
                ["thisyear"] = new ThisYearPeriod(today),
                ["lastyear"] = new LastYearPeriod(today),
            };
        }

        public IPeriod[] Periods => moments.Values.ToArray();

        public bool CanProvide(string text) => moments.ContainsKey(text.ToLower());
        public IPeriod Provide(string text) => moments[text.ToLower()];
    }
    #endregion

    #region period
    public class AlwaysPeriod : IPeriod
    {
        public static AlwaysPeriod Default { get; } = new AlwaysPeriod();

        public string Text => string.Empty;
        public string Label => string.Empty;

        public DateTime? MinDate => null;
        public DateTime? MaxDate => null;
    }

    public class BeforePeriod : IPeriod
    {
        public string Text => $"<{Period.Text}";
        public string Label => $"< {Period.Label}";

        public DateTime? MinDate => null;
        public DateTime? MaxDate => Period.MaxDate;

        public IPeriod Period { get; }

        public BeforePeriod(IPeriod period) => Period = period ?? AlwaysPeriod.Default;
    }

    public class AfterPeriod : IPeriod
    {
        public string Text => $">{Period.Text}";
        public string Label => $"> {Period.Label}";

        public DateTime? MinDate => Period.MinDate;
        public DateTime? MaxDate => null;

        public IPeriod Period { get; }

        public AfterPeriod(IPeriod period) => Period = period ?? AlwaysPeriod.Default;
    }

    public class IntervalPeriod : IPeriod
    {
        public string Text => $"{MinPeriod.Text}..{MaxPeriod.Text}";
        public string Label => $"{MinPeriod.Label}..{MaxPeriod.Label}";

        public DateTime? MinDate => MinPeriod.MinDate;
        public DateTime? MaxDate => MaxPeriod.MaxDate;

        public IPeriod MinPeriod { get; }
        public IPeriod MaxPeriod { get; }

        public IntervalPeriod(IPeriod minPeriod, IPeriod maxPeriod)
        {
            MinPeriod = minPeriod ?? AlwaysPeriod.Default;
            MaxPeriod = maxPeriod ?? AlwaysPeriod.Default;
        }
    }

    public class DatePeriod : IPeriod
    {
        public string Text => $"{Date:d}";
        public string Label => $"{Date:D}";

        public DateTime? MinDate => Date;
        public DateTime? MaxDate => Date;

        public DateTime Date { get; }

        public DatePeriod(DateTime date) => Date = date;
    }

    public class YearPeriod : IPeriod
    {
        public string Text => $"{Year}";
        public string Label => $"Year {Year}";

        public DateTime? MinDate => new DateTime(Year, 1, 1);
        public DateTime? MaxDate => new DateTime(Year, 12, 31);

        public int Year { get; }

        public YearPeriod(ushort year) => Year = year;
    }

    public class TodayPeriod : IPeriod
    {
        public string Text => "today";
        public string Label => "Today";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public TodayPeriod() : this(DateTime.Today)
        {
        }
        public TodayPeriod(DateTime today) : base()
        {
            MinDate = MaxDate = today.Date;
        }
    }

    public class YesterdayPeriod : IPeriod
    {
        public string Text => "yesterday";
        public string Label => "Yesterday";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public YesterdayPeriod() : this(DateTime.Today)
        {
        }
        public YesterdayPeriod(DateTime today) : base()
        {
            MinDate = MaxDate = today.Date.AddDays(-1);
        }
    }

    public class ThisWeekPeriod : IPeriod
    {
        public string Text => "thisweek";
        public string Label => "This week";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public ThisWeekPeriod() : this(DateTime.Today)
        {
        }
        public ThisWeekPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddDays(-6);
            MaxDate = today.Date;
        }
    }

    public class LastWeekPeriod : IPeriod
    {
        public string Text => "lastweek";
        public string Label => "Last week";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public LastWeekPeriod() : this(DateTime.Today)
        {
        }
        public LastWeekPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddDays(-13);
            MaxDate = today.Date.AddDays(-7);
        }
    }

    public class ThisMonthPeriod : IPeriod
    {
        public string Text => "thismonth";
        public string Label => "This month";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public ThisMonthPeriod() : this(DateTime.Today)
        {
        }
        public ThisMonthPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddMonths(-1).AddDays(1);
            MaxDate = today.Date;
        }
    }

    public class LastMonthPeriod : IPeriod
    {
        public string Text => "lastmonth";
        public string Label => "last month";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public LastMonthPeriod() : this(DateTime.Today)
        {
        }
        public LastMonthPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddMonths(-2).AddDays(1);
            MaxDate = today.Date.AddMonths(-1);
        }
    }

    public class ThisYearPeriod : IPeriod
    {
        public string Text => "thisyear";
        public string Label => "This year";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public ThisYearPeriod() : this(DateTime.Today)
        {
        }
        public ThisYearPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddYears(-1).AddDays(1);
            MaxDate = today.Date;
        }
    }

    public class LastYearPeriod : IPeriod
    {
        public string Text => "lastyear";
        public string Label => "last year";

        public DateTime? MinDate { get; }
        public DateTime? MaxDate { get; }

        public LastYearPeriod() : this(DateTime.Today)
        {
        }
        public LastYearPeriod(DateTime today) : base()
        {
            MinDate = today.Date.AddYears(-2).AddDays(1);
            MaxDate = today.Date.AddYears(-1);
        }
    }
    #endregion
}
