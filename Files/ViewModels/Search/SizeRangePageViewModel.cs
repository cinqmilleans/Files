using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISettingSearchPageViewModel
    {
        SizeRange Range { get; set; }
        ICommand ClearCommand { get; }
        IEnumerable<ISizeRangeLink> Links { get; }
    }

    public interface ISizeRangeLink : INotifyPropertyChanged
    {
        bool IsSelected { get; }
        SizeRange Range { get; }
        ICommand ToggleCommand { get; }
    }

    public class SizeRangePageViewModel : SettingSearchPageViewModel, ISizeRangePageViewModel
    {
        public override string Glyph { get; } = "\xF0E2";
        public override string Title { get; } = "Size";

        public override bool HasValue => !Range.Equals(SizeRange.All) && !Range.Equals(SizeRange.None);

        public SizeRange Range
        {
            get => Navigator.Settings.FileSize;
            set => Navigator.Settings.FileSize = value;
        }

        public ICommand ClearCommand { get; }
        public IEnumerable<ISizeRangeLink> Links { get; }

        public SizeRangePageViewModel(ISearchNavigatorViewModel navigator) : base(navigator)
        {
            ClearCommand = new RelayCommand(() => Range = SizeRange.All);
            Links = new List<SizeRange>
            {
                SizeRange.Empty,
                SizeRange.Tiny,
                SizeRange.Small,
                SizeRange.Medium,
                SizeRange.Large,
                SizeRange.VeryLarge,
                SizeRange.Huge
            }.Select(range => new SizeRangeLink(this, range));

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchSettings.FileSize))
            {
                if (Range.Equals(SizeRange.None))
                {
                    Range = SizeRange.All;
                }
                else
                {
                    OnPropertyChanged(nameof(Range));
                    OnPropertyChanged(nameof(HasValue));
                }
            }
        }

        private class SizeRangeLink : ObservableObject, ISizeRangeLink
        {
            private readonly SizeRangePageViewModel viewModel;

            public SizeRange Range { get; set; }

            private bool isSelected = false;
            public bool IsSelected
            {
                get => isSelected;
                set => SetProperty(ref isSelected, value);
            }

            public ICommand ToggleCommand { get; set; }

            public SizeRangeLink(SizeRangePageViewModel viewModel, SizeRange range)
            {
                this.viewModel = viewModel;

                IsSelected = GetIsSelected();
                Range = range;
                ToggleCommand = new RelayCommand(Toggle);

                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }

            private bool GetIsSelected()
                => viewModel.Range.IsNamed && viewModel.HasValue && viewModel.Range.Contains(Range);

            private void Toggle()
            {
                if (!viewModel.HasValue)
                {
                    viewModel.Range = Range;
                }
                else if (IsSelected)
                {
                    if (Range.Equals(SizeRange.Empty))
                    {
                        viewModel.Range -= new SizeRange(Size.MinValue, new Size(1));
                    }
                    else
                    {
                        viewModel.Range -= Range;
                    }
                }
                else if (viewModel.Range.IsNamed)
                {
                    viewModel.Range += Range;
                }
                else
                {
                    viewModel.Range = Range;
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
