using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class FolderSearchOptionMenu : UserControl
    {
        public IFolderSearchCriteriaViewModel[] BaseFilters { get; }
        public ObservableCollection<IFolderSearchCriteriaViewModel> UserFilters { get; }
        public ObservableCollection<IFolderSearchCriteriaViewModel> FreeFilters { get; }

        public FolderSearchOptionMenu()
        {
            InitializeComponent();

            var ViewModeFactory = new FolderSearchCriteriaViewModelFactory();


            var criteria = FolderSearchOption.Default.Filters
                .Where(filter => filter is IFolderSearchCriteria)
                .Cast<IFolderSearchCriteria>()
                .Select(c => ViewModeFactory.ToViewModel(c))
                .ToList();

            BaseFilters = new IFolderSearchCriteriaViewModel[]
            {
                criteria.First(f => f.Key == "creationDate")
            };
            UserFilters = new ObservableCollection<IFolderSearchCriteriaViewModel>();
            FreeFilters = new ObservableCollection<IFolderSearchCriteriaViewModel>(criteria.Except(BaseFilters));
        }
    }

    public class FolderSearchCriteriaViewModelFactory
    {
        public IFolderSearchCriteriaViewModel ToViewModel(IFolderSearchCriteria filter)
        {
            if (filter is DateFolderSearchFilter dateFolderSearchFilter)
            {
                return new DateFolderSearchFilterViewModel(dateFolderSearchFilter);
            };
            return null;
        }
    }

    public interface IFolderSearchCriteriaViewModel : INotifyPropertyChanged
    {
        public string Key { get; }
        public string Label { get; }
    }

    public class FolderSearchFilterViewModel<T> : ObservableObject, IFolderSearchCriteriaViewModel where T : IFolderSearchCriteria
    {
        public string Key => Filter.Key;
        public string Label => Filter.Label;

        protected T Filter { get; }

        public FolderSearchFilterViewModel(T filter) : base()
        {
            Filter = filter;
        }
    }

    public class DateFolderSearchFilterViewModel : FolderSearchFilterViewModel<DateFolderSearchFilter>
    {
        public PeriodViewModel[] Periods { get; } = new PeriodViewModel[]
        {
            new PeriodViewModel(" ", DateFolderSearchFilter.Periods.None),
            new PeriodViewModel("A day ago", DateFolderSearchFilter.Periods.DayAgo),
            new PeriodViewModel("A week ago", DateFolderSearchFilter.Periods.WeekAgo),
            new PeriodViewModel("A month ago", DateFolderSearchFilter.Periods.MonthAgo),
            new PeriodViewModel("A year ago", DateFolderSearchFilter.Periods.YearAgo),
            new PeriodViewModel("Custom", DateFolderSearchFilter.Periods.Custom),
        };

        public DateFolderSearchFilterViewModel(DateFolderSearchFilter filter) : base(filter)
        {
        }

        public PeriodViewModel Period
        {
            get => Periods.First(period => period.Value == Filter.Period);
            set
            {
                Filter.Period = value.Value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCustomPeriod));
                Filter.Comparator = GetComparator();
            }
        }

        public DateTimeOffset? MinDate
        {
            get => Filter.MinDate;
            set
            {
                SetProperty(ref Filter.MinDate, value);
                Filter.Comparator = GetComparator();
            }
        }
        public DateTimeOffset? MaxDate
        {
            get => Filter.MaxDate;
            set
            {
                SetProperty(ref Filter.MaxDate, value);
                Filter.Comparator = GetComparator();
            }
        }

        public bool IsCustomPeriod =>
            Filter.Period == DateFolderSearchFilter.Periods.Custom;

        private DateFolderSearchFilter.Comparators GetComparator()
        {
            if (Filter.Period != DateFolderSearchFilter.Periods.Custom)
            {
                return DateFolderSearchFilter.Comparators.None;
            }
            if (Filter.MinDate.HasValue && Filter.MaxDate.HasValue)
            {
                return DateFolderSearchFilter.Comparators.Between;
            }
            if (Filter.MinDate.HasValue)
            {
                return DateFolderSearchFilter.Comparators.After;
            }
            if (Filter.MaxDate.HasValue)
            {
                return DateFolderSearchFilter.Comparators.Before;
            }
            return DateFolderSearchFilter.Comparators.None;
        }
    }

    public class PeriodViewModel : ObservableObject
    {
        public string Label { get; }
        public DateFolderSearchFilter.Periods Value { get; }

        public PeriodViewModel(string label, DateFolderSearchFilter.Periods value)
        {
            Label = label;
            Value = value;
        }
    }

    public class FolderSearchFilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DateTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is DateFolderSearchFilterViewModel)
            {
                return DateTemplate;
            }
            return null;
        }
    }
}
