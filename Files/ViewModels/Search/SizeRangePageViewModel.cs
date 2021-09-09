using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISettingSearchPageViewModel
    {
        bool IsAll { get; }
        SizeRange Range { get; set; }

        ISizeRangePageLink AllLink { get; }
        ISizeRangePageLink EmptyLink { get; }
        ISizeRangePageLink TinyLink { get; }
        ISizeRangePageLink SmallLink { get; }
        ISizeRangePageLink MediumLink { get; }
        ISizeRangePageLink LargeLink { get; }
        ISizeRangePageLink VeryLargeLink { get; }
        ISizeRangePageLink HugeLink { get; }
    }

    public interface ISizeRangePageLink
    {
        SizeRange Range { get; }
        ICommand SelectCommand { get; }
    }

    public class SizeRangePageViewModel : SettingSearchPageViewModel, ISizeRangePageViewModel
    {
        public override string Glyph { get; } = "\xF0E2";
        public override string Title { get; } = "Size";

        public bool IsAll => Navigator.Settings.FileSize.Equals(SizeRange.All);

        public SizeRange Range
        {
            get => Navigator.Settings.FileSize;
            set
            {
                value ??= new();
                if (!Navigator.Settings.FileSize.Equals(value))
                {
                    Navigator.Settings.FileSize = value;
                }
            }
        }

        public ISizeRangePageLink AllLink { get; }
        public ISizeRangePageLink EmptyLink { get; }
        public ISizeRangePageLink TinyLink { get; }
        public ISizeRangePageLink SmallLink { get; }
        public ISizeRangePageLink MediumLink { get; }
        public ISizeRangePageLink LargeLink { get; }
        public ISizeRangePageLink VeryLargeLink { get; }
        public ISizeRangePageLink HugeLink { get; }

        public SizeRangePageViewModel(ISearchNavigatorViewModel navigator) : base(navigator)
        {
            AllLink = GetLink(SizeRange.All);
            EmptyLink = GetLink(SizeRange.Empty);
            TinyLink = GetLink(SizeRange.Tiny);
            SmallLink = GetLink(SizeRange.Small);
            MediumLink = GetLink(SizeRange.Medium);
            LargeLink = GetLink(SizeRange.Large);
            VeryLargeLink = GetLink(SizeRange.VeryLarge);
            HugeLink = GetLink(SizeRange.Huge);

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;

            ISizeRangePageLink GetLink(SizeRange range) => new SizeRangePageLink
            {
                Range = range,
                SelectCommand = new RelayCommand(() => Range = range),
            };
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileSize")
            {
                Range = Navigator.Settings.FileSize;
                OnPropertyChanged(nameof(Range));
                OnPropertyChanged(nameof(IsAll));
            }
        }

        private class SizeRangePageLink : ISizeRangePageLink
        {
            public SizeRange Range { get; set; }
            public ICommand SelectCommand { get; set; }
        }
    }
}
