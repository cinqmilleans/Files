using Files.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls
{
    public sealed partial class PaneControl : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(PaneViewModel), typeof(PaneControl), new PropertyMetadata(null));

        public PaneViewModel ViewModel
        {
            get => (PaneViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public PaneControl() => InitializeComponent();

        private void Root_Loading(FrameworkElement sender, object args)
        {
            if (PreviewPane != null)
            {
                //PreviewPane.Model = ViewModel?.PaneHolder?.ActivePaneOrColumn?.SlimContentPage?.PreviewPaneViewModel;
            }
            PreviewPane?.Model?.UpdateSelectedItemPreview();
        }
    }
}
