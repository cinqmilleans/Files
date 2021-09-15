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

        IDateRange Range { get; set; }
        ICommand ClearCommand { get; }
        IEnumerable<IDateRangeLink> Links { get; }
    }

    public interface IDateRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        NameDateRange Range { get; }
        ICommand ToggleCommand { get; }
    }

    public abstract class DateRangePageViewModel : SettingSearchPageViewModel, IDateRangePageViewModel
    {
        private readonly Date today;
        private readonly IDateRangeFactory factory;

        private readonly string settingName;

        public override string Glyph { get; } = "\xE163";
        public override string Title { get; } = "Date";
        public abstract string LongTitle { get; }

        public override bool HasValue => Range.MinDate > Date.MinValue || Range.MaxDate < Date.Today;

        public abstract IDateRange Range { get; set; }

        public DateTimeOffset? MinDateTime
        {
            get
            {
                var minDate = Range.MinDate;
                return minDate != Date.MinValue ? minDate.offset : null;
            }
            set
            {
                var minDate = value.HasValue ? new Date(value.Value.DateTime) : Date.MinValue;
                Range = factory.Build(minDate, Range.MaxDate);
            }
        }
        public DateTimeOffset? MaxDateTime
        {
            get
            {
                var maxDate = Range.MaxDate;
                return maxDate != Date.MaxValue ? maxDate.offset : null;
            }
            set
            {
                var maxDate = value.HasValue ? new Date(value.Value.DateTime) : today;
                Range = factory.Build(Range.MinDate, maxDate);
            }
        }

        public ICommand ClearCommand { get; }
        public IEnumerable<IDateRangeLink> Links { get; }

        public DateRangePageViewModel(string settingName, ISearchNavigatorViewModel navigator) : base(navigator)
        {
            today = Date.Today;
            factory = new DateRangeFactory(today);

            this.settingName = settingName;
            ClearCommand = new RelayCommand(() => Range = NameDateRange.All);
            Links = Enum.GetValues(typeof(NameDateRange.Names)).Cast<NameDateRange.Names>().Reverse().Select(name => new DateRangeLink(this, name));

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;
        }

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

            public NameDateRange Range { get; set; }

            private bool isSelected = false;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }

            public ICommand ToggleCommand { get; set; }

            public DateRangeLink(DateRangePageViewModel viewModel, NameDateRange.Names name)
            {
                this.viewModel = viewModel;

                IsSelected = GetIsSelected();
                Range = new NameDateRange(name);
                ToggleCommand = new RelayCommand(Toggle);

                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            private bool GetIsSelected()
                => viewModel.Range is NameDateRange range && viewModel.HasValue && range.MinName <= Range.MinName && range.MaxName >= Range.MaxName;

            private void Toggle()
            {
                if (Range.MinName == NameDateRange.Names.Today)
                {
                }
                if (Range.MaxName == NameDateRange.Names.Older)
                {
                }

                if (!viewModel.HasValue)
                {
                    viewModel.Range = Range;
                }
                else if (IsSelected)
                {
                    Deselect();
                }
                else
                {
                    Select();
                }
            }
            private void Select()
            {
                if (viewModel.Range is NameDateRange range)
                {
                    if (Range.MinName < range.MinName)
                    {
                        var a = new NameDateRange(Range.MinName, range.MaxName);
                        viewModel.Range = a; // new NameDateRange(Range.MinName, range.MaxName);
                    }
                    else if (Range.MaxName > range.MaxName)
                    {
                        var b = new NameDateRange(range.MinName, Range.MaxName);
                        viewModel.Range = b; // new NameDateRange(range.MinName, Range.MaxName);
                    }
                }
                else
                {
                    viewModel.Range = Range;
                }
            }
            private void Deselect()
            {
                if (viewModel.Range is NameDateRange range)
                {
                    if (range.MinName == Range.MinName && range.MinName < NameDateRange.Names.Today)
                    {
                        viewModel.Range = new NameDateRange(range.MinName + 1, range.MaxName);
                    }
                    else if (range.MaxName == Range.MaxName && range.MaxName > NameDateRange.Names.Older)
                    {
                        viewModel.Range = new NameDateRange(range.MinName, range.MaxName - 1);
                    }
                    else
                    {
                        viewModel.Range = NameDateRange.All;
                    }
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

        public override IDateRange Range
        {
            get => Navigator.Settings.Created;
            set => Navigator.Settings.Created = value ?? NameDateRange.All;
        }

        public CreatedPageViewModel(ISearchNavigatorViewModel navigator) : base(nameof(ISearchSettings.Created), navigator) {}
    }

    public class ModifiedPageViewModel : DateRangePageViewModel
    {
        public override string Title { get; } = "Modified";
        public override string LongTitle { get; } = "Last modification date";

        public override IDateRange Range
        {
            get => Navigator.Settings.Modified;
            set => Navigator.Settings.Modified = value ?? NameDateRange.All;
        }

        public ModifiedPageViewModel(ISearchNavigatorViewModel navigator) : base(nameof(ISearchSettings.Modified), navigator) {}
    }
}
