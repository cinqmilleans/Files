using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Files.Filesystem.Search
{
    public interface ISettings
    {
        ILocation Location { get; }
        IFilter Filter { get; }
    }

    public interface ILocation : INotifyPropertyChanged
    {
        ObservableCollection<string> Folders { get; }
        LocationOptions Options { get; set; }
    }

    [Flags]
    public enum LocationOptions : ushort
    {
        None = 0x0000,
        SubFolders = 0x0001,
        SystemFiles = 0x0002,
        CompressedFiles = 0x0004,
    }

    public interface IFilterCollection : ICollection<IFilter>, IFilter, INotifyCollectionChanged
    {
    }

    public interface IFilter : INotifyPropertyChanged
    {
        bool IsEmpty { get; }

        string Glyph { get; }
        string ShortLabel { get; }
        string FullLabel { get; }

        void Clear();

        string ToAdvancedQuerySyntax();
    }

    public interface IOperatorFilter : IFilter
    {
        IFilter SubFilter { get; set; }
    }

    public interface IDateRangeFilter : IFilter
    {
        DateRange Range { get; set; }
    }
    public interface ISizeRangeFilter : IFilter
    {
        SizeRange Range { get; set; }
    }

    public class Settings : ObservableObject, ISettings
    {
        public static Settings Default { get; } = new();

        public ILocation Location { get; } = new Location();
        public IFilter Filter { get; } = new AndFilter();
    }

    public class Location : ObservableObject, ILocation
    {
        public ObservableCollection<string> Folders { get; } = new ObservableCollection<string>();

        private LocationOptions options = LocationOptions.SubFolders;
        public LocationOptions Options
        {
            get => options;
            set => SetProperty(ref options, value);
        }
    }

    public class AndFilter : ObservableCollection<IFilter>, IFilterCollection
    {
        public bool IsEmpty => Count == 0;

        public string Glyph => "\xE168";
        public string ShortLabel => "And";
        public string FullLabel => "And Operator";

        public AndFilter() : base() {}
        public AndFilter(IEnumerable<IFilter> filters) : base(filters) {}
        public AndFilter(IList<IFilter> filters) : base(filters) {}

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsEmpty)));
        }

        public string ToAdvancedQuerySyntax()
        {
            var queries = Items.Where(filter => !filter.IsEmpty).Select(filter => ToQuery(filter));
            return string.Join(' ', queries);

            static string ToQuery(IFilter filter)
            {
                var query = filter.ToAdvancedQuerySyntax().Trim();
                return query.Contains(' ') ? query : $"({query})";
            }
        }
    }
    public class OrFilter : ObservableCollection<IFilter>, IFilterCollection
    {
        public bool IsEmpty => Count == 0;

        public string Glyph => "\xE168";
        public string ShortLabel => "Or";
        public string FullLabel => "Or Operator";

        public OrFilter() : base() {}
        public OrFilter(IEnumerable<IFilter> filters) : base(filters) {}
        public OrFilter(IList<IFilter> filters) : base(filters) {}

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsEmpty)));
        }

        public string ToAdvancedQuerySyntax()
        {
            var queries = Items.Where(filter => !filter.IsEmpty).Select(filter => ToQuery(filter));
            return string.Join(' ', queries);

            static string ToQuery(IFilter filter)
            {
                var query = filter.ToAdvancedQuerySyntax().Trim();
                return query.Contains(" or ") ? query : $"({query})";
            }
        }
    }
    public class NotFilter : ObservableObject, IOperatorFilter
    {
        public bool IsEmpty => subFilter is null;

        public string Glyph => "\xE168";
        public string ShortLabel => "Not";
        public string FullLabel => "Not Operator";

        private IFilter subFilter;
        public IFilter SubFilter
        {
            get => subFilter;
            set
            {
                if (SetProperty(ref subFilter, value))
                {
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public void Clear() => SubFilter = null;

        public string ToAdvancedQuerySyntax() => subFilter switch
        {
            null => string.Empty,
            _ => $"not({subFilter.ToAdvancedQuerySyntax()})"
        };
    }

    public abstract class DateRangeSetting : ObservableObject, IDateRangeFilter
    {
        public bool IsEmpty => !range.Equals(DateRange.Always);

        public string Glyph => "\xE163";
        public abstract string ShortLabel { get; }
        public virtual string FullLabel => ShortLabel;
        protected abstract string QueryLabel { get; }

        private DateRange range = DateRange.Always;
        public DateRange Range
        {
            get => range;
            set
            {
                if (SetProperty(ref range, value))
                {
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public void Clear() => Range = DateRange.Always;

        public string ToAdvancedQuerySyntax()
        {
            var (min, max) = range;
            bool hasMin = min > Date.MinValue;
            bool hasMax = max < Date.Today;

            return (hasMin, hasMax) switch
            {
                (false, false) => string.Empty,
                _ when min == max => $"{min:yyyyMMdd}",
                (false, _) => $"{QueryLabel}:<={max:yyyyMMdd}",
                (_, false) => $"{QueryLabel}:>={min:yyyyMMdd}",
                _ => $"{QueryLabel}:{min:yyyyMMdd}..{max:yyyyMMdd}"
            };
        }
    }
    public class CreatedSetting : DateRangeSetting
    {
        public override string ShortLabel => "Created";
        public override string FullLabel => "Creation Date";
        protected override string QueryLabel => "System.ItemDate";
    }
    public class ModifiedSetting : DateRangeSetting
    {
        public override string ShortLabel => "Modified";
        public override string FullLabel => "Last modified Date";
        protected override string QueryLabel => "System.DateModified";
    }

    public class FileSizeSetting : ObservableObject, ISizeRangeFilter
    {
        public bool IsEmpty => !range.Equals(SizeRange.All);

        public string Glyph => "\xE163";
        public string ShortLabel => "Size";
        public string FullLabel => "File Size";

        private SizeRange range = SizeRange.All;
        public SizeRange Range
        {
            get => range;
            set
            {
                if (SetProperty(ref range, value))
                {
                    OnPropertyChanged(nameof(IsEmpty));
                }
            }
        }

        public void Clear() => Range = SizeRange.All;

        public string ToAdvancedQuerySyntax()
        {
            var (min, max) = range;
            bool hasMin = min > Size.MinValue;
            bool hasMax = max < Size.MaxValue;

            return (hasMin, hasMax) switch
            {
                (false, false) => string.Empty,
                _ when min == max => $"{min:yyyyMMdd}",
                (false, _) => $"System.Size:<={max:yyyyMMdd}",
                (_, false) => $"System.Size:>={min:yyyyMMdd}",
                _ => $"System.Size:{min:yyyyMMdd}..{max:yyyyMMdd}"
            };
        }
    }
}
