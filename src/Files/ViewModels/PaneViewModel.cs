using Files.Enums;
using Files.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;

namespace Files.ViewModels
{
    public interface IPaneViewModel : INotifyPropertyChanged
    {
        bool HasContent { get; }

        bool IsPreviewSelected { get; set; }
        bool IsSearchSelected { get; set; }
    }

    public class PaneViewModel : ObservableObject, IPaneViewModel
    {
        private readonly IPaneSettingsService settings = Ioc.Default.GetService<IPaneSettingsService>();

        public bool HasContent => settings.Content is not PaneContents.None;

        public bool IsPreviewSelected
        {
            get => settings.Content is PaneContents.Preview;
            set => SetContent(PaneContents.Preview);
        }

        public bool IsSearchSelected
        {
            get => settings.Content is PaneContents.Search;
            set => SetContent(PaneContents.Search);
        }

        public PaneViewModel() => settings.PropertyChanged += Settings_PropertyChanged;

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(IPaneSettingsService.Content))
            {
                OnPropertyChanged(nameof(HasContent));
                OnPropertyChanged(nameof(IsPreviewSelected));
                OnPropertyChanged(nameof(IsSearchSelected));
            }
        }

        private void SetContent(PaneContents content)
        {
            var old = settings.Content;
            if (old is PaneContents.None)
            {
                settings.Content = content;
            }
            else
            {
                settings.Content = PaneContents.None;
            }
        }
    }
}
