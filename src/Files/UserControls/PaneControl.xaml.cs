using Files.Models;
using Files.Services;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class PaneControl : UserControl
    {
        public readonly IUserSettingsService userSettingsService = Ioc.Default.GetService<IUserSettingsService>();

        public static readonly DependencyProperty PaneProperty =
            DependencyProperty.Register(nameof(Pane), typeof(Panes), typeof(PaneControl), new PropertyMetadata(Panes.None));

        public Panes Pane
        {
            get => (Panes)GetValue(PaneProperty);
            set
            {
                SetValue(PaneProperty, value);
                OnPr
            }
        }

        private Panes PanesPreview => Panes.Preview;
        private Panes PanesSearch => Panes.Search;

        private bool CanLoadPane => Pane is not Panes.None;
        private bool CanLoadPreviewPane => Pane is Panes.Preview;

        public PaneControl() => InitializeComponent();

        private void Splitter_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Pane is Panes.None || Switch.CurrentCase.Content is null)
            {
                return;
            }

            if (PreviewPane.IsHorizontal)
            {
                UserSettingsService.PreviewPaneSettingsService.PreviewPaneSizeHorizontalPx = PreviewPane.ActualHeight;
            }
            else
            {
                UserSettingsService.PreviewPaneSettingsService.PreviewPaneSizeVerticalPx = PreviewPane.ActualWidth;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
