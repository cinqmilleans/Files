using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IDateRangePageViewModel : ISettingSearchPageViewModel
    {
        string LongTitle { get; }

        DateTimeOffset? MinDateTime { get; set; }
        DateTimeOffset? MaxDateTime { get; set; }

        DateRange Range { get; set; }
        ICommand ClearCommand { get; }
        IReadOnlyList<IDateRangeLink> Links { get; }
    }

    public interface IDateRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        DateRange Range { get; }
        ICommand ToggleCommand { get; }
    }

    public abstract class DateRangePageViewModel : SettingSearchPageViewModel, IDateRangePageViewModel
    {
        private readonly string settingName;

        public override string Glyph { get; } = "\xE163";
        public override string Title { get; } = "Date";
        public abstract string LongTitle { get; }

        public override bool HasValue => !Range.Equals(DateRange.None) && !Range.Equals(DateRange.Always);

        public abstract DateRange Range { get; set; }

        public DateTimeOffset? MinDateTime
        {
            get
            {
                var minDate = Range.MinDate;
                return minDate != Date.MinValue ? minDate.Offset : null;
            }
            set
            {
                var minDate = value.HasValue ? new Date(value.Value.DateTime) : Date.MinValue;
                Range = new(minDate, Range.MaxDate);
            }
        }
        public DateTimeOffset? MaxDateTime
        {
            get
            {
                var maxDate = Range.MaxDate;
                return maxDate != Date.MaxValue ? maxDate.Offset : null;
            }
            set
            {
                var maxDate = value.HasValue ? new Date(value.Value.DateTime) : DateRange.Today.MinDate;
                Range = new(Range.MinDate, maxDate);
            }
        }

        public ICommand ClearCommand { get; }

        private readonly Lazy<IReadOnlyList<IDateRangeLink>> links;
        public IReadOnlyList<IDateRangeLink> Links => links.Value;

        public DateRangePageViewModel(string settingName, ISearchNavigatorViewModel navigator) : base(navigator)
        {
            this.settingName = settingName;
            ClearCommand = new RelayCommand(() => Range = DateRange.Always);
            links = new(GetLinks);
            navigator.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private IReadOnlyList<IDateRangeLink> GetLinks() => new List<DateRange>
        {
            DateRange.Today,
            DateRange.Yesterday,
            DateRange.ThisWeek,
            DateRange.LastWeek,
            DateRange.ThisMonth,
            DateRange.LastMonth,
            DateRange.ThisYear,
            DateRange.Older
        }.Select(range => new DateRangeLink(this, range)).Cast<IDateRangeLink>().ToList().AsReadOnly();

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == settingName)
            {
                OnPropertyChanged(nameof(Range));
                OnPropertyChanged(nameof(MinDateTime));
                OnPropertyChanged(nameof(MaxDateTime));
                OnPropertyChanged(nameof(HasValue));
            }
        }

        private class DateRangeLink : ObservableObject, IDateRangeLink
        {
            private readonly DateRangePageViewModel viewModel;

            public DateRange Range { get; set; }

            private bool isSelected = false;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }

            public ICommand ToggleCommand { get; set; }

            public DateRangeLink(DateRangePageViewModel viewModel, DateRange range)
            {
                this.viewModel = viewModel;

                IsSelected = GetIsSelected();
                Range = range;
                ToggleCommand = new RelayCommand(Toggle);

                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            private bool GetIsSelected()
                => viewModel.Range.IsNamed && viewModel.HasValue && viewModel.Range.Contains(Range);

            private void Toggle()
            {
                if (!viewModel.HasValue)
                {
                    viewModel.Range = Range;
                }
                else if (IsSelected)
                {
                    viewModel.Range -= Range;
                }
                else if (viewModel.Range.IsNamed)
                {
                    viewModel.Range += Range;
                }
                else
                {
                    viewModel.Range = Range;
                }
            }

            private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IDateRangePageViewModel.Range))
                {
                    IsSelected = GetIsSelected();
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }

    public class CreatedPageViewModel : DateRangePageViewModel
    {
        public override string Title { get; } = "Created";
        public override string LongTitle { get; } = "Creation date";

        public override DateRange Range
        {
            get => Navigator.Settings.Created;
            set => Navigator.Settings.Created = value;
        }

        public CreatedPageViewModel(ISearchNavigatorViewModel navigator)
            : base(nameof(ISearchSettings.Created), navigator) {}
    }

    public class ModifiedPageViewModel : DateRangePageViewModel
    {
        public override string Title { get; } = "Modified";
        public override string LongTitle { get; } = "Last modification date";

        public override DateRange Range
        {
            get => Navigator.Settings.Modified;
            set => Navigator.Settings.Modified = value;
        }

        public ModifiedPageViewModel(ISearchNavigatorViewModel navigator)
            : base(nameof(ISearchSettings.Modified), navigator) {}
    }
}
