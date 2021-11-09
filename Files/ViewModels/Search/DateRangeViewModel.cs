﻿using Files.Extensions;
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
    public interface IDateRangePageViewModel : IMultiSearchPageViewModel
    {
        new IDateRangePickerViewModel Picker { get; }
    }

    public interface IDateRangePickerViewModel : IPickerViewModel
    {
        string Description { get; set; }
        string Label { get; }
        DateRange Range { get; set; }

        DateTimeOffset Today { get; }
        DateTimeOffset? MinOffset { get; set; }
        DateTimeOffset? MaxOffset { get; set; }

        IReadOnlyList<IDateRangeLink> Links { get; }
    }

    public interface IDateRangeHeader : ISearchFilterHeader
    {
        IDateRangeFilter GetFilter(DateRange range);
    }

    public interface IDateRangeContext : ISearchFilterContext
    {
    }

    public interface IDateRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        string Label { get; }
        ICommand ToggleCommand { get; }
    }

    public class CreatedHeader : SearchFilterHeader<CreatedFilter>, IDateRangeHeader
    {
        IDateRangeFilter IDateRangeHeader.GetFilter(DateRange range) => GetFilter(range);
        public CreatedFilter GetFilter(DateRange range) => new(range);
    }
    public class ModifiedHeader : SearchFilterHeader<ModifiedFilter>, IDateRangeHeader
    {
        IDateRangeFilter IDateRangeHeader.GetFilter(DateRange range) => GetFilter(range);
        public ModifiedFilter GetFilter(DateRange range) => new(range);
    }
    public class AccessedHeader : SearchFilterHeader<AccessedFilter>, IDateRangeHeader
    {
        IDateRangeFilter IDateRangeHeader.GetFilter(DateRange range) => GetFilter(range);
        public AccessedFilter GetFilter(DateRange range) => new(range);
    }

    public class DateRangeContext : SearchFilterContext<IDateRangeFilter>, IDateRangeContext
    {
        public override string Label => GetFilter().Range.ToString("n");

        public DateRangeContext(ISearchPageContext parentPageContext, IDateRangeFilter filter) : base(parentPageContext, filter) {}
    }

    public class DateRangePageViewModel : ObservableObject, IDateRangePageViewModel
    {
        private readonly ISearchPageContext context;

        ISearchPageNavigator ISearchPageViewModel.Navigator => context;

        public IEnumerable<ISearchFilterHeader> Headers { get; } = new List<ISearchFilterHeader>
        {
            new CreatedHeader(),
            new ModifiedHeader(),
            new AccessedHeader(),
        };

        private ISearchFilterHeader header;
        public ISearchFilterHeader Header
        {
            get => header;
            set
            {
                if (SetProperty(ref header, value))
                {
                    Picker.Description = header.Description;
                    Save();
                }
            }
        }

        IPickerViewModel ISearchPageViewModel.Picker => Picker;
        public IDateRangePickerViewModel Picker { get; }

        public DateRangePageViewModel(ISearchPageContext context) : this(context, new CreatedFilter())
        {
        }
        public DateRangePageViewModel(ISearchPageContext context, IDateRangeFilter filter)
        {
            this.context = context;

            header = filter switch
            {
                CreatedFilter => Headers.First(h => h is CreatedHeader),
                ModifiedFilter => Headers.First(h => h is ModifiedHeader),
                AccessedFilter => Headers.First(h => h is AccessedHeader),
                _ => Headers.First(),
            };

            Picker = new PickerViewModel(Save);
            Picker.Description = header?.Description;
            if (filter is not null)
            {
                Picker.Range = filter.Range;
            }
        }

        private void Save()
        {
            if (Picker.IsEmpty)
            {
                context.Save(null);
            }
            else
            {
                var header = Header as IDateRangeHeader;
                var filter = header.GetFilter(Picker.Range);
                context.Save(filter);
            }
        }

        private class PickerViewModel : ObservableObject, IDateRangePickerViewModel
        {
            private readonly Action saveAction;

            public bool IsEmpty => range == DateRange.Always;

            public DateTimeOffset Today => Date.Today.Offset;

            private DateRange range = DateRange.Always;
            public DateRange Range
            {
                get => range;
                set
                {
                    if (SetProperty(ref range, value))
                    {
                        OnPropertyChanged(nameof(IsEmpty));
                        OnPropertyChanged(nameof(Label));
                        OnPropertyChanged(nameof(MinOffset));
                        OnPropertyChanged(nameof(MaxOffset));

                        links.ForEach(link => link.UpdateProperties());

                        saveAction();
                    }
                }
            }

            private string description;
            public string Description
            {
                get => description;
                set => SetProperty(ref description, value);
            }

            public string Label => range.ToString("N");

            public DateTimeOffset? MinOffset
            {
                get => range.MinDate > Date.MinValue ? range.MinDate.Offset : null;
                set
                {
                    var minDate = value.HasValue ? new Date(value.Value.DateTime) : Date.MinValue;
                    Range = new DateRange(minDate, range.MaxDate);
                }
            }
            public DateTimeOffset? MaxOffset
            {
                get => range.MaxDate < Date.MaxValue ? range.MaxDate.Offset : null;
                set
                {
                    var maxDate = value.HasValue ? new Date(value.Value.DateTime) : Date.MaxValue;
                    Range = new DateRange(range.MinDate, maxDate);
                }
            }

            private readonly IReadOnlyList<DateRangeLink> links;
            public IReadOnlyList<IDateRangeLink> Links => links;

            public ICommand ClearCommand { get; }

            public PickerViewModel(Action saveAction)
            {
                this.saveAction = saveAction;

                links = new List<DateRange>
                {
                    DateRange.Today,
                    DateRange.Yesterday,
                    DateRange.ThisWeek,
                    DateRange.LastWeek,
                    DateRange.ThisMonth,
                    DateRange.LastMonth,
                    DateRange.ThisYear,
                    DateRange.Older,
                }.Select(range => new DateRangeLink(this, range)).ToList().AsReadOnly();

                ClearCommand = new RelayCommand(Clear);
            }

            public void Clear() => Range = DateRange.Always;

            private class DateRangeLink : ObservableObject, IDateRangeLink
            {
                private readonly IDateRangePickerViewModel picker;
                private readonly DateRange range;

                public bool IsSelected
                {
                    get => !picker.IsEmpty && picker.Range.IsNamed && picker.Range.Contains(range);
                    set
                    {
                        if (IsSelected != value)
                        {
                            Toggle();
                        }
                    }
                }

                public string Label => range.ToString("n");

                public ICommand ToggleCommand { get; }

                public DateRangeLink(IDateRangePickerViewModel picker, DateRange range)
                {
                    this.picker = picker;
                    this.range = range;
                    ToggleCommand = new RelayCommand(Toggle);
                }

                public void UpdateProperties() => OnPropertyChanged(nameof(IsSelected));

                private void Toggle()
                {
                    if (picker.IsEmpty)
                    {
                        picker.Range = range;
                    }
                    else if (IsSelected)
                    {
                        picker.Range -= range;
                    }
                    else if (picker.Range.IsNamed)
                    {
                        picker.Range += range;
                    }
                    else
                    {
                        picker.Range = range;
                    }
                }
            }
        }
    }
}
