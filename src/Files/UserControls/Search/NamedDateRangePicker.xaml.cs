using Files.Filesystem.Search;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class NamedDateRangePicker : Page
    {
        public static readonly DependencyProperty DateRangeProperty =
            DependencyProperty.Register(nameof(Range), typeof(DateRange), typeof(NamedDateRangePicker), new PropertyMetadata(DateRange.Always));

        public DateRange Range
        {
            get => (DateRange)GetValue(DateRangeProperty);
            set => SetValue(DateRangeProperty, value);
        }

        private IList<IPeriod> Periods { get; } = new List<IPeriod>
        {
            new Period(DateRange.Today),
            new Period(DateRange.Yesterday),
            new Period(DateRange.ThisWeek),
            new Period(DateRange.LastWeek),
            new Period(DateRange.ThisMonth),
            new Period(DateRange.LastMonth),
            new Period(DateRange.ThisYear),
            new Period(DateRange.Older),
        };

        public NamedDateRangePicker() => InitializeComponent();

        private class Period : IPeriod
        {
            public DateRange Range { get; }
            public string Label { get; }
            public bool IsSelected { get; } = false;

            public Period(DateRange range)
            {
                Range = range;
                Label = Range.ToLabel().MinValue;
            }
        }
    }

    public interface IPeriod
    {
        DateRange Range { get; }
        string Label { get; }
        bool IsSelected { get; }
    }
}
