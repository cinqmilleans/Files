using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Windows.Storage.Search;

namespace Files.Filesystem.Search
{
    [Flags]
    public enum SearchSettingLocations : ushort
    {
        None = 0x0000,
        SubFolders = 0x0001,
        SystemFiles = 0x0002,
        CompressedFiles = 0x0004,
    }

    public interface ISearchSettings : INotifyPropertyChanged
    {
        SearchSettingLocations Location { get; set; }

        DateRange Created { get; set; }
        DateRange Modified { get; set; }
        SizeRange FileSize { get; set; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        public static SearchSettings Default { get; } = new();

        private SearchSettingLocations location = SearchSettingLocations.SubFolders;
        public SearchSettingLocations Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

        private DateRange created = DateRange.Always;
        public DateRange Created
        {
            get => created;
            set => SetProperty(ref created, value);
        }

        private DateRange modified = DateRange.Always;
        public DateRange Modified
        {
            get => modified;
            set => SetProperty(ref modified, value);
        }

        private SizeRange fileSize = SizeRange.All;
        public SizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value);
        }

        public FolderDepth ToFolderDeepth()
            => Location.HasFlag(SearchSettingLocations.SubFolders) ? FolderDepth.Deep : FolderDepth.Shallow;

        public string ToAdvancedQuerySyntax()
        {
            return string.Join(' ', new List<string>
            {
                ToSettingQuery("System.ItemDate", $"{Created:q}"),
                ToSettingQuery("System.DateModified", $"{Modified:q}"),
                ToSettingQuery("System.Size", $"{FileSize:q}"),
            }.Where(s => !string.IsNullOrEmpty(s)));

            static string ToSettingQuery(string name, string value)
                => string.IsNullOrEmpty(value) ? string.Empty : $"{name}:{value}";
        }
    }
}
