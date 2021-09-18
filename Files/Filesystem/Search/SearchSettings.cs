using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Text;

namespace Files.Filesystem.Search
{
    [Flags]
    public enum SearchSettingLocation : ushort
    {
        None = 0x0000,
        SubFolders = 0x0001,
        SystemFiles = 0x0002,
        CompressedFiles = 0x0004,
    }

    public interface ISearchSettings : INotifyPropertyChanged
    {
        SearchSettingLocation Location { get; set; }

        IDateRange Created { get; set; }
        IDateRange Modified { get; set; }
        SizeRange FileSize { get; set; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public static SearchSettings Default { get; } = new();

        private SearchSettingLocation location = SearchSettingLocation.SubFolders;
        public SearchSettingLocation Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

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

        private SizeRange fileSize = SizeRange.All;
        public SizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value);
        }

        public string ToAdvancedQuerySyntax()
        {
            var query = new StringBuilder();

            string fileSizeQuery = fileSize.ToString("q");
            if (!string.IsNullOrEmpty(fileSizeQuery))
                query.Append(" System.ItemSize:" + fileSizeQuery);

            return query.ToString().Trim();
        }
    }
}
