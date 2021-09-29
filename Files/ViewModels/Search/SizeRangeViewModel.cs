﻿using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISizeRangeViewModel : IFilterViewModel
    {
        new ISizeRangeFilter Filter { get; }

        IReadOnlyList<ISizeRangeLink> Links { get; }
    }

    public interface ISizeRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        SizeRange Range { get; }
        ICommand ToggleCommand { get; }
    }

    public class SizeRangeViewModel : FilterViewModel, ISizeRangeViewModel
    {
        public new ISizeRangeFilter Filter { get; }

        private readonly Lazy<IReadOnlyList<ISizeRangeLink>> links;
        public IReadOnlyList<ISizeRangeLink> Links => links.Value;

        public SizeRangeViewModel(ISizeRangeFilter filter)
        {
            Filter = filter;
            links = new(GetLinks);
            filter.PropertyChanged += Filter_PropertyChanged;
        }

        private IReadOnlyList<ISizeRangeLink> GetLinks() => new List<SizeRange>
        {
            SizeRange.Empty,
            SizeRange.Tiny,
            SizeRange.Small,
            SizeRange.Medium,
            SizeRange.Large,
            SizeRange.VeryLarge,
            SizeRange.Huge,
        }.Select(range => new SizeRangeLink(Filter, range)).Cast<ISizeRangeLink>().ToList().AsReadOnly();

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISizeRangeFilter.Range))
            {
                if (Filter.Range.Equals(SizeRange.None))
                {
                    Filter.Range = SizeRange.All;
                }
            }
        }

        private class SizeRangeLink : ObservableObject, ISizeRangeLink
        {
            private readonly ISizeRangeFilter filter;

            public SizeRange Range { get; set; }

            private bool isSelected = false;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }

            public ICommand ToggleCommand { get; set; }

            public SizeRangeLink(ISizeRangeFilter filter, SizeRange range)
            {
                this.filter = filter;

                IsSelected = GetIsSelected();
                Range = range;
                ToggleCommand = new RelayCommand(Toggle);

                filter.PropertyChanged += Filter_PropertyChanged;
            }

            private bool GetIsSelected()
                => !filter.IsEmpty && filter.Range.IsNamed && filter.Range.Contains(Range);

            private void Toggle()
            {
                if (filter.IsEmpty)
                {
                    filter.Range = Range;
                }
                else if (IsSelected)
                {
                    if (Range.Equals(SizeRange.Empty))
                    {
                        filter.Range -= new SizeRange(Size.MinValue, new Size(1));
                    }
                    else
                    {
                        filter.Range -= Range;
                    }
                }
                else if (filter.Range.IsNamed)
                {
                    filter.Range += Range;
                }
                else
                {
                    filter.Range = Range;
                }
            }

            private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ISizeRangeFilter.Range))
                {
                    IsSelected = GetIsSelected();
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
