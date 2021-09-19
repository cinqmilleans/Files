using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface ILocationViewModel : INotifyPropertyChanged
    {
        public bool UseSubFolders { get; set; }
        public bool UseSystemFiles { get; set; }
        public bool UseCompressedFiles { get; set; }
    }

    public class LocationViewModel : ObservableObject, ILocationViewModel
    {
        private readonly ISearchSettings setting;

        public bool UseSubFolders
        {
            get => setting.Location.HasFlag(SearchSettingLocations.SubFolders);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocations.SubFolders;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocations.SubFolders;
                }
            }
        }

        public bool UseSystemFiles
        {
            get => setting.Location.HasFlag(SearchSettingLocations.SystemFiles);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocations.SystemFiles;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocations.SystemFiles;
                }
            }
        }

        public bool UseCompressedFiles
        {
            get => setting.Location.HasFlag(SearchSettingLocations.CompressedFiles);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocations.CompressedFiles;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocations.CompressedFiles;
                }
            }
        }

        public LocationViewModel(ISearchSettings setting)
        {
            this.setting = setting;
            setting.PropertyChanged += Setting_PropertyChanged;
        }

        private void Setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchSettings.Location))
            {
                OnPropertyChanged(nameof(UseSubFolders));
                OnPropertyChanged(nameof(UseSystemFiles));
                OnPropertyChanged(nameof(UseCompressedFiles));
            }
        }
    }
}
