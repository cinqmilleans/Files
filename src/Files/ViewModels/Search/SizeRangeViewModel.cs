using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISearchPageViewModel
    {
        new ISizeRangePickerViewModel Picker { get; }
    }

    public interface ISizeRangePickerViewModel : IPickerViewModel
    {
        SizeRange Range { get; set; }

        string Label { get; }
        string Description { get; }
    }

    public interface ISizeRangeContext : ISearchFilterContext
    {
    }

    public class SizeRangeContext : SearchFilterContext<ISizeRangeFilter>, ISizeRangeContext
    {
        public override string Label => GetFilter().Range.ToString("n");

        public SizeRangeContext(ISearchPageContext parentPageContext, ISizeRangeFilter filter) : base(parentPageContext, filter) {}
    }

    public class SizeRangeHeader : SearchFilterHeader<SizeRangeFilter>
    {
        public SizeRangeFilter GetFilter(SizeRange range) => new(range);
    }

    public class SizeRangePageViewModel : ObservableObject, ISizeRangePageViewModel
    {
        private readonly ISearchPageContext context;

        ISearchPageNavigator ISearchPageViewModel.Navigator => context;

        public ISearchFilterHeader Header { get; } = new SizeRangeHeader();

        IPickerViewModel ISearchPageViewModel.Picker => Picker;
        public ISizeRangePickerViewModel Picker { get; }

        public SizeRangePageViewModel(ISearchPageContext context) : this(context, new SizeRangeFilter())
        {
        }
        public SizeRangePageViewModel(ISearchPageContext context, ISizeRangeFilter filter)
        {
            this.context = context;

            Picker = new PickerViewModel(Save);
            if (filter is not null)
            {
                Picker.Range = filter.Range;
            }
        }

        private void Save() => context.Save(!Picker.IsEmpty ? new SizeRangeFilter(Picker.Range) : null);

        private class PickerViewModel : ObservableObject, ISizeRangePickerViewModel
        {
            private readonly Action saveAction;

            public bool IsEmpty => range == SizeRange.All;

            private SizeRange range = SizeRange.All;
            public SizeRange Range
            {
                get => range;
                set
                {
                    if (value.Equals(SizeRange.None))
                    {
                        value = SizeRange.All;
                    }
                    if (SetProperty(ref range, value))
                    {
                        OnPropertyChanged(nameof(IsEmpty));
                        OnPropertyChanged(nameof(Label));

                        saveAction();
                    }
                }
            }

            public string Label => range.ToString("N");
            public string Description { get; }

            public ICommand ClearCommand { get; }

            public PickerViewModel(Action saveAction)
            {
                this.saveAction = saveAction;

                Description = new SizeRangeHeader().Description;
                Range = range;

                ClearCommand = new RelayCommand(Clear);
            }

            public void Clear() => Range = SizeRange.All;
        }
    }
}
