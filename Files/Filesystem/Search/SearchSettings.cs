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
        ISizeRange FileSize { get; set; }
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

        private ISizeRange fileSize = NameSizeRange.All;
        public ISizeRange FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value ?? NameSizeRange.All);
        }

        public string ToAdvancedQuerySyntax()
        {
            var query = new StringBuilder();

            //query.Append(" " + ToDateRangeQuery("System.ItemDate", Created)).;
            //query.Append(" " + ToDateRangeQuery("System.DateModified", Modified));
            //query.Append(" " + ToSizeRangeQuery("System.ItemSize", FileSize));

            return query.ToString().Trim();

            //static string ToDateRangeQuery(string option, IDateRange range) => (range.MinDate, range.MaxDate) switch
            //{
            //    (Date.MinValue, Date.MaxValue) => string.Empty,
            //    (Date.MinValue, _) => $"{option}:>{range.MinDate:yyyyMMdd}",
            //    (_, Date.MaxValue) => $"{option}:<{range.MaxDate:yyyyMMdd}",
            //    _ => $"{option}:{range.MinDate:yyyyMMdd}..{range.MaxDate:yyyyMMdd}",
            //};
            //static string ToSizeRangeQuery(string option, ISizeRange range) => (range.MinSize, range.MaxSize) switch
            //{
            //    (Size.MinValue, Size.MaxValue) => string.Empty,
            //    (Size.MinValue, _) => $"{option}:>{range.MinSize.Bytes}",
            //    (_, Size.MaxValue) => $"{option}:<{range.MaxSize.Bytes}",
            //    _ => $"{option}:{range.MinSize.Bytes}..{range.MaxSize.Bytes}",
            //};
        }
    }
}
