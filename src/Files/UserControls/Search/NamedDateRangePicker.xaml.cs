using Files.Extensions;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class NamedDateRangePicker : UserControl
    {
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register(nameof(Range), typeof(DateRange), typeof(NamedDateRangePicker), new PropertyMetadata(DateRange.Always));

        public DateRange Range
        {
            get => (DateRange)GetValue(RangeProperty);
            set
            {
                if (value.Equals(DateRange.None))
                {
                    value = DateRange.Always;
                }
                if (Range != value)
                {
                    SetValue(RangeProperty, value);
                    NamedRanges.ForEach(link => link.UpdateProperties());
                }
            }
        }

        private IEnumerable<NamedDateRange> NamedRanges { get; }

        public NamedDateRangePicker()
        {
            InitializeComponent();

            NamedRanges = new List<DateRange>
            {
                DateRange.Today,
                DateRange.Yesterday,
                DateRange.ThisWeek,
                DateRange.LastWeek,
                DateRange.ThisMonth,
                DateRange.LastMonth,
                DateRange.ThisYear,
                DateRange.Older,
            }.Select(range => new NamedDateRange(this, range)).ToList();
        }

        private class NamedDateRange : ObservableObject, INamedDateRange
        {
            private readonly NamedDateRangePicker picker;
            private readonly DateRange range;

            public string Name => range.ToString("N");

            public bool IsSelected
            {
                get => picker.Range != DateRange.Always && picker.Range.IsNamed && picker.Range.Contains(range);
                set
                {
                    if (IsSelected != value)
                    {
                        Toggle();
                    }
                }
            }

            public NamedDateRange(NamedDateRangePicker picker, DateRange range) => (this.picker, this.range) = (picker, range);

            public void UpdateProperties() => OnPropertyChanged(nameof(IsSelected));

            private void Toggle()
            {
                if (picker.Range == DateRange.Always)
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

    public interface INamedDateRange : INotifyPropertyChanged
    {
        string Name { get; }
        bool IsSelected { get; set; }
    }
}
