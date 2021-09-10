using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IDateRangePageViewModel : ISettingSearchPageViewModel
    {
        DateRange Range { get; set; }

        IDateRangeLink AlwaysLink { get; }
        IDateRangeLink TodayLink { get; }
        IDateRangeLink YesterdayLink { get; }
        IDateRangeLink ThisWeekLink { get; }
        IDateRangeLink LastWeekLink { get; }
        IDateRangeLink ThisMonthLink { get; }
        IDateRangeLink LastMonthLink { get; }
        IDateRangeLink ThisYearLink { get; }
        IDateRangeLink LastYearLink { get; }
    }

    public interface IDateRangeLink
    {
        DateRange Range { get; }
        ICommand SelectCommand { get; }
    }

    public abstract class DateRangePageViewModel : SettingSearchPageViewModel, IDateRangePageViewModel
    {
        private readonly string propertyName;

        public override string Glyph { get; } = "\xE163";
        public override string Title { get; } = "Creation Date";

        public override bool HasValue => !AlwaysLink.Range.Equals(Range);

        public abstract DateRange Range { get; set; }

        public IDateRangeLink AlwaysLink { get; }
        public IDateRangeLink TodayLink { get; }
        public IDateRangeLink YesterdayLink { get; }
        public IDateRangeLink ThisWeekLink { get; }
        public IDateRangeLink LastWeekLink { get; }
        public IDateRangeLink ThisMonthLink { get; }
        public IDateRangeLink LastMonthLink { get; }
        public IDateRangeLink ThisYearLink { get; }
        public IDateRangeLink LastYearLink { get; }

        public DateRangePageViewModel(string propertyName, ISearchNavigatorViewModel navigator) : base(navigator)
        {
            this.propertyName = propertyName;

            var today = Date.Today;

            AlwaysLink = GetLink(NamedDateRange.Names.Always);
            TodayLink = GetLink(NamedDateRange.Names.Today);
            YesterdayLink = GetLink(NamedDateRange.Names.Yesterday);
            ThisWeekLink = GetLink(NamedDateRange.Names.ThisWeek);
            LastWeekLink = GetLink(NamedDateRange.Names.LastWeek);
            ThisMonthLink = GetLink(NamedDateRange.Names.ThisMonth);
            LastMonthLink = GetLink(NamedDateRange.Names.LastMonth);
            ThisYearLink = GetLink(NamedDateRange.Names.ThisYear);
            LastYearLink = GetLink(NamedDateRange.Names.LastYear);

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;

            IDateRangeLink GetLink(NamedDateRange.Names name) => new DateRangeLink
            {
                Range = new NamedDateRange(name, today),
                SelectCommand = new RelayCommand(() => Range = new NamedDateRange(name)),
            };
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == propertyName)
            {
                OnPropertyChanged(nameof(Range));
                OnPropertyChanged(nameof(HasValue));
            }
        }

        private class DateRangeLink : IDateRangeLink
        {
            public DateRange Range { get; set; }
            public ICommand SelectCommand { get; set; }
        }
    }

    public class CreatedPageViewModel : DateRangePageViewModel
    {
        public override string Title { get; } = "Creation Date";

        public override DateRange Range
        {
            get => Navigator.Settings.Created;
            set => Navigator.Settings.Created = value ?? new();
        }

        public CreatedPageViewModel(ISearchNavigatorViewModel navigator) : base("Created", navigator) {}
    }

    public class ModifiedPageViewModel : DateRangePageViewModel
    {
        public override string Title { get; } = "Last Modification Date";

        public override DateRange Range
        {
            get => Navigator.Settings.Created;
            set => Navigator.Settings.Created = value ?? new();
        }

        public ModifiedPageViewModel(ISearchNavigatorViewModel navigator) : base("Modified", navigator) { }
    }
}
