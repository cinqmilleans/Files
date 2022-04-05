using Files.Helpers.XamlHelpers;
using Files.ViewModels;
using Windows.Devices.Input;
using Windows.System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls
{
    public sealed partial class PathControl : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(NavToolbarViewModel), typeof(NavigationToolbar), new PropertyMetadata(null));

        public NavToolbarViewModel ViewModel
        {
            get => (NavToolbarViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public PathControl() => InitializeComponent();

        private void VisiblePath_Loaded(object _, RoutedEventArgs e)
        {
            // AutoSuggestBox won't receive focus unless it's fully loaded
            VisiblePath.Focus(FocusState.Programmatic);
            DependencyObjectHelpers.FindChild<TextBox>(VisiblePath)?.SelectAll();
        }

        private void VisiblePath_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key is VirtualKey.Escape)
            {
                ViewModel.IsEditModeEnabled = false;
            }
        }

        private void VisiblePath_LostFocus(object sender, RoutedEventArgs e)
        {
            var element = FocusManager.GetFocusedElement();
            if (element is not AppBarButton && element is Control control)
            {
                if (control.FocusState is not FocusState.Programmatic and not FocusState.Keyboard)
                {
                    ViewModel.IsEditModeEnabled = false;
                }
                else if (ViewModel.IsEditModeEnabled)
                {
                    VisiblePath.Focus(FocusState.Programmatic);
                }
            }
        }

        private void ClickablePath_Click(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType is PointerDeviceType.Mouse)
            {
                PointerPoint point = e.GetCurrentPoint(RootControl);
                if (point.Properties.IsMiddleButtonPressed)
                {
                    return;
                }
            }
            ViewModel.IsEditModeEnabled = true;
        }
    }
}
