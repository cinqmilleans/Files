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
        public static SearchSettings Default { get; } = new SearchSettings();

        private DateRange created = new DateRange();
        public DateRange Created
        {
            get => created;
            set => SetProperty(ref created, value ?? new DateRange());
        }

        private DateRange modified = new DateRange();
        public DateRange Modified
        {
            get => modified;
            set => SetProperty(ref modified, value ?? new DateRange());
        }

        private SizeRange fileSize = new SizeRange();
        public SizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value ?? new SizeRange());
        }
    }
}
