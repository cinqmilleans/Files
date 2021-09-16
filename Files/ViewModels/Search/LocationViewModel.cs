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
            get => setting.Location.HasFlag(SearchSettingLocation.SubFolders);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocation.SubFolders;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocation.SubFolders;
                }
            }
        }

        public bool UseSystemFiles
        {
            get => setting.Location.HasFlag(SearchSettingLocation.SystemFiles);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocation.SystemFiles;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocation.SystemFiles;
                }
            }
        }

        public bool UseCompressedFiles
        {
            get => setting.Location.HasFlag(SearchSettingLocation.CompressedFiles);
            set
            {
                if (value)
                {
                    setting.Location |= SearchSettingLocation.CompressedFiles;
                }
                else
                {
                    setting.Location &= ~SearchSettingLocation.CompressedFiles;
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
