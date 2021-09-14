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
    public interface ISizeRangePageViewModel : ISettingSearchPageViewModel
    {
        ISizeRange Range { get; set; }
        ICommand ClearCommand { get; }
        IEnumerable<ISizeRangeLink> Links { get; }
    }

    public interface ISizeRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        NameSizeRange Range { get; }
        ICommand ToggleCommand { get; }
    }

    public class SizeRangePageViewModel : SettingSearchPageViewModel, ISizeRangePageViewModel
    {
        public override string Glyph { get; } = "\xF0E2";
        public override string Title { get; } = "Size";

        public override bool HasValue => Range.MinSize != Size.MinValue || Range.MaxSize != Size.MaxValue;

        public ISizeRange Range
        {
            get => Navigator.Settings.FileSize;
            set => Navigator.Settings.FileSize = value;
        }

        public ICommand ClearCommand { get; }
        public IEnumerable<ISizeRangeLink> Links { get; }

        public SizeRangePageViewModel(ISearchNavigatorViewModel navigator) : base(navigator)
        {
            ClearCommand = new RelayCommand(() => Range = NameSizeRange.All);
            Links = Enum.GetValues(typeof(NameSizeRange.Names)).Cast<NameSizeRange.Names>().Select(name => new SizeRangeLink(this, name));

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchSettings.FileSize))
            {
                OnPropertyChanged(nameof(Range));
                OnPropertyChanged(nameof(HasValue));
            }
        }

        private class SizeRangeLink : ObservableObject, ISizeRangeLink
        {
            private readonly SizeRangePageViewModel viewModel;

            public NameSizeRange Range { get; set; }

            private bool isSelected = false;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }

            public ICommand ToggleCommand { get; set; }

            public SizeRangeLink(SizeRangePageViewModel viewModel, NameSizeRange.Names name)
            {
                this.viewModel = viewModel;

                IsSelected = GetIsSelected();
                Range = new NameSizeRange(name);
                ToggleCommand = new RelayCommand(Toggle);

                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            private bool GetIsSelected()
                => viewModel.Range is NameSizeRange range && viewModel.HasValue && range.MinName <= Range.MinName && range.MaxName >= Range.MaxName;

            private void Toggle()
            {
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
                if (viewModel.Range is NameSizeRange range)
                {
                    if (Range.MinName < range.MinName)
                    {
                        viewModel.Range = new NameSizeRange(Range.MinName, range.MaxName);
                    }
                    else if (Range.MaxName > range.MaxName)
                    {
                        viewModel.Range = new NameSizeRange(range.MinName, Range.MaxName);
                    }
                }
                else
                {
                    viewModel.Range = Range;
                }
            }
            private void Deselect()
            {
                if (viewModel.Range is NameSizeRange range)
                {
                    if (range.MinName == Range.MinName && range.MinName < NameSizeRange.Names.Huge)
                    {
                        viewModel.Range = new NameSizeRange(range.MinName + 1, range.MaxName);
                    }
                    else if (range.MaxName == Range.MaxName && range.MaxName > NameSizeRange.Names.Empty)
                    {
                        viewModel.Range = new NameSizeRange(range.MinName, range.MaxName - 1);
                    }
                    else
                    {
                        viewModel.Range = NameSizeRange.All;
                    }
                }
            }

            private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(ISizeRangePageViewModel.Range))
                {
                    IsSelected = GetIsSelected();
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
    }
}
