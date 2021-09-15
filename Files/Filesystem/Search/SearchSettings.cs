using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        IDateRange Created { get; set; }
        IDateRange Modified { get; set; }
        ISizeRange FileSize { get; set; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public static SearchSettings Default { get; } = new();

        private IDateRange created = NameDateRange.All;
        public IDateRange Created
        {
            get => created;
            set => SetProperty(ref created, value ?? NameDateRange.All);
        }

        private IDateRange modified = NameDateRange.All;
        public IDateRange Modified
        {
            get => modified;
            set => SetProperty(ref modified, value ?? NameDateRange.All);
        }

        private ISizeRange fileSize = NameSizeRange.All;
        public ISizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value ?? NameSizeRange.All);
        }
    }
}
