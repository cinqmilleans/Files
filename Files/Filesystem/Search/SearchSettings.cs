using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        DateRange Created { get; set; }
        DateRange Modified { get; set; }
        SizeRange FileSize { get; set; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public static SearchSettings Default { get; } = new();

        private DateRange created = new();
        public DateRange Created
        {
            get => created;
            set => SetProperty(ref created, value ?? new());
        }

        private DateRange modified = new();
        public DateRange Modified
        {
            get => modified;
            set => SetProperty(ref modified, value ?? new());
        }

        private SizeRange fileSize = new();
        public SizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value ?? new());
        }
    }
}
