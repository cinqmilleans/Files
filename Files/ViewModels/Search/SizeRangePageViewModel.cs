using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISizeRangePageViewModel : ISettingSearchPageViewModel
    {
        SizeRange Range { get; set; }

        ISizeRangeLink AllLink { get; }
        ISizeRangeLink EmptyLink { get; }
        ISizeRangeLink TinyLink { get; }
        ISizeRangeLink SmallLink { get; }
        ISizeRangeLink MediumLink { get; }
        ISizeRangeLink LargeLink { get; }
        ISizeRangeLink VeryLargeLink { get; }
        ISizeRangeLink HugeLink { get; }
    }

    public interface ISizeRangeLink
    {
        SizeRange Range { get; }
        ICommand SelectCommand { get; }
    }

    public class SizeRangePageViewModel : SettingSearchPageViewModel, ISizeRangePageViewModel
    {
        public override string Glyph { get; } = "\xF0E2";
        public override string Title { get; } = "Size";

        public override bool HasValue => !AllLink.Range.Equals(Range);

        public SizeRange Range
        {
            get => Navigator.Settings.FileSize;
            set => Navigator.Settings.FileSize = value ?? new();
        }

        public ISizeRangeLink AllLink { get; }
        public ISizeRangeLink EmptyLink { get; }
        public ISizeRangeLink TinyLink { get; }
        public ISizeRangeLink SmallLink { get; }
        public ISizeRangeLink MediumLink { get; }
        public ISizeRangeLink LargeLink { get; }
        public ISizeRangeLink VeryLargeLink { get; }
        public ISizeRangeLink HugeLink { get; }

        public SizeRangePageViewModel(ISearchNavigatorViewModel navigator) : base(navigator)
        {
            AllLink = GetLink(NamedSizeRange.Names.All);
            EmptyLink = GetLink(NamedSizeRange.Names.Empty);
            TinyLink = GetLink(NamedSizeRange.Names.Tiny);
            SmallLink = GetLink(NamedSizeRange.Names.Small);
            MediumLink = GetLink(NamedSizeRange.Names.Medium);
            LargeLink = GetLink(NamedSizeRange.Names.Large);
            VeryLargeLink = GetLink(NamedSizeRange.Names.VeryLarge);
            HugeLink = GetLink(NamedSizeRange.Names.Huge);

            navigator.Settings.PropertyChanged += Settings_PropertyChanged;

            ISizeRangeLink GetLink(NamedSizeRange.Names name) => new SizeRangeLink
            {
                Range = new NamedSizeRange(name),
                SelectCommand = new RelayCommand(() => Range = new NamedSizeRange(name)),
            };
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileSize")
            {
                OnPropertyChanged(nameof(Range));
                OnPropertyChanged(nameof(HasValue));
            }
        }

        private class SizeRangeLink : ISizeRangeLink
        {
            public SizeRange Range { get; set; }
            public ICommand SelectCommand { get; set; }
        }
    }
}
