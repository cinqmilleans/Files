using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class SizeRangePicker : UserControl
    {
        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register(nameof(Range), typeof(SizeRange), typeof(SizeRangeSlider), new PropertyMetadata(null));

        public SizeRange Range
        {
            get => (SizeRange)GetValue(RangeProperty);
            set
            {
                if (!value.Equals(Range))
                {
                    SetValue(RangeProperty, value);
                    Item.Update();
                }
            }
        }

        private RangeItem Item { get; }

        public SizeRangePicker()
        {
            InitializeComponent();
            Item = new RangeItem(this);
            SetValue(RangeProperty, new SizeRange());
        }

        public class UnitItem : ObservableObject
        {
            public static IList<UnitItem> Items = new List<UnitItem>
            {
                new UnitItem(Size.Units.Byte),
                new UnitItem(Size.Units.Kibi),
                new UnitItem(Size.Units.Mebi),
                new UnitItem(Size.Units.Gibi),
                new UnitItem(Size.Units.Tebi),
                new UnitItem(Size.Units.Pebi),
            };

            public Size.Units Unit { get; }
            public string Label { get; }

            private UnitItem(Size.Units unit)
            {
                Unit = unit;
                Label = new Size(1, unit).ToString("U");
            }
        }

        private class RangeItem : ObservableObject
        {
            private readonly SizeRangePicker picker;

            public IList<UnitItem> Units => UnitItem.Items;

            public SizeRange Range
            {
                get => picker.Range;
                set => picker.Range = value;
            }

            public double MinSizeValue
            {
                get => Range.MinSize.Value;
                set
                {
                    if (Range.MinSize.Value != value)
                    {
                        Range = new SizeRange(new Size(value, MinSizeUnit), Range.MaxSize);
                    }
                }
            }
            public Size.Units MinSizeUnit
            {
                get => Range.MinSize.Unit;
                set
                {
                    if (Range.MinSize.Unit != value)
                    {
                        Range = new SizeRange(new Size(MinSizeValue, value), Range.MaxSize);
                    }
                }
            }

            public double MaxSizeValue
            {
                get => Range.MaxSize.Value;
                set
                {
                    if (Range.MaxSize.Value != value)
                    {
                        Range = new SizeRange(Range.MinSize, new Size(value, MaxSizeUnit));
                    }
                }
            }
            public Size.Units MaxSizeUnit
            {
                get => Range.MaxSize.Unit;
                set
                {
                    if (Range.MaxSize.Unit != value)
                    {
                        Range = new SizeRange(Range.MinSize, new Size(MaxSizeValue, value));
                    }
                }
            }

            public RangeItem(SizeRangePicker picker) => this.picker = picker;

            public void Update()
            {
                OnPropertyChanged(nameof(MinSizeValue));
                OnPropertyChanged(nameof(MinSizeUnit));
                OnPropertyChanged(nameof(MaxSizeValue));
                OnPropertyChanged(nameof(MaxSizeUnit));
            }
        }
    }
}
