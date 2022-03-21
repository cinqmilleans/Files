using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Backend.Item
{
    internal class FileItemViewModel : ObservableObject, IFileItemViewModel, IDisposable
    {
        private readonly IFileItem item;

        public string Path => item.Path;
        public string Name => item.Name;
        public string Extension => item.Extension;

        public bool IsArchive => item.FileAttribute.HasFlag(FileAttributes.Archive);
        public bool IsCompressed => item.FileAttribute.HasFlag(FileAttributes.Compressed);
        public bool IsDevice => item.FileAttribute.HasFlag(FileAttributes.Device);
        public bool IsDirectory => item.FileAttribute.HasFlag(FileAttributes.Directory);
        public bool IsEncrypted => item.FileAttribute.HasFlag(FileAttributes.Encrypted);
        public bool IsHidden => item.FileAttribute.HasFlag(FileAttributes.Hidden);
        public bool IsOffline => item.FileAttribute.HasFlag(FileAttributes.Offline);
        public bool IsReadOnly => item.FileAttribute.HasFlag(FileAttributes.ReadOnly);
        public bool IsSystem => item.FileAttribute.HasFlag(FileAttributes.System);
        public bool IsTemporary => item.FileAttribute.HasFlag(FileAttributes.Temporary);
        public bool IsExecutable => item.FileAttribute.HasFlag(FileAttributes.Executable);

        public ByteSize Size => item.Size;

        public DateTime DateCreated => item.DateCreated;
        public DateTime DateModified => item.DateModified;
        public DateTime DateAccessed => item.DateAccessed;
        public DateTime DateDeleted => (item as IRecycleBinItem)?.DateDeleted ?? default;

        public bool IsShortcutItem => item is IShortcutItem;
        public bool IsSymbolicLinkShortcutItem => Shortcut?.IsSymbolicLink ?? false;
        public bool IsUrlShortcutItem => Shortcut?.IsUrl ?? false;
        public bool IsLibraryItem => item is ILibraryItem;
        public bool IsRecycleBinItem => item is IRecycleBinItem;
        public bool IsFtpItem => item is IFtpItem;
        public bool IsZipItem => item is IZipItem;

        public IShortcutViewModel? Shortcut { get; }
        public ILibraryViewModel? Library { get; }

        public BitmapImage? MainImage => item.MainImage;
        public BitmapImage? OverlayImage => item.OverlayImage;

        public FileItemViewModel(IFileItem item)
        {
            this.item = item;

            if (item is INotifyPropertyChanged observable)
            {
                observable.PropertyChanged += Observable_PropertyChanged;
            }
            if (item is IShortcutItem shortcutItem)
            {
                Shortcut = new ShortcutViewModel(shortcutItem.Shortcut);
            }
            if (item is ILibraryItem libraryItem)
            {
                Library = new LibraryViewModel(libraryItem.Library);
            }
        }

        private void Observable_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IFileItem.Size):
                    OnPropertyChanged(nameof(Size));
                    break;
                case nameof(IFileItem.MainImage):
                    OnPropertyChanged(nameof(MainImage));
                    break;
                case nameof(IFileItem.OverlayImage):
                    OnPropertyChanged(nameof(MainImage));
                    break;
            }
        }

        public void Dispose()
        {
            if (item is INotifyPropertyChanged observable)
            {
                observable.PropertyChanged -= Observable_PropertyChanged;
            }
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
