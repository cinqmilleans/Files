using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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
        DateRange Range { get; set; }

        string Label { get; }
        string Description { get; set; }
    }

    public interface IDateRangeHeader : ISearchFilterHeader
    {
        IDateRangeFilter GetFilter(DateRange range);
    }

    public interface IDateRangeContext : ISearchFilterContext
    {
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

                        saveAction();
                    }
                }
            }

            public string Label => range.ToString("N");

            private string description;
            public string Description
            {
                get => description;
                set => SetProperty(ref description, value);
            }

            public ICommand ClearCommand { get; }

            public PickerViewModel(Action saveAction)
            {
                this.saveAction = saveAction;

                ClearCommand = new RelayCommand(Clear);
            }

            public void Clear() => Range = DateRange.Always;
        }
    }
}
