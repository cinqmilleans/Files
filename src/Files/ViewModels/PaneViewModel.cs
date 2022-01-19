using Files.Models;
using Files.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;

namespace Files.ViewModels
{
    public class PaneViewModel : ObservableObject
    {
        private readonly SidebarViewModel sidebarViewModel;

        private readonly IPaneSettingsService paneSettingsService
            = Ioc.Default.GetService<IUserSettingsService>()?.PaneSettingsService;

        public bool HasContent => paneSettingsService.Content is not PaneContents.None;
        public bool HasPreview => paneSettingsService.Content is PaneContents.Preview && !(IsHomePage && IsMultiPaneActive);

        public bool IsPreviewSelected
        {
            get => paneSettingsService.Content is PaneContents.Preview;
            set => paneSettingsService.Content = !IsPreviewSelected ? PaneContents.Preview : PaneContents.None;
        }

        private bool IsHomePage => !(sidebarViewModel?.PaneHolder?.ActivePane?.InstanceViewModel?.IsPageTypeNotHome ?? true);
        private bool IsMultiPaneActive => sidebarViewModel?.PaneHolder?.IsMultiPaneActive ?? false;

        public PaneViewModel(SidebarViewModel sidebarViewModel)
        {
            this.sidebarViewModel = sidebarViewModel;
            paneSettingsService.PropertyChanged += PaneSettingsService_PropertyChanged;
        }

        private void PaneSettingsService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(IPaneSettingsService.Content))
            {
                OnPropertyChanged(nameof(HasContent));
                OnPropertyChanged(nameof(HasPreview));
            }
        }
    }
}
