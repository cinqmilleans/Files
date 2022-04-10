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
            DependencyProperty.Register(nameof(ViewModel), typeof(NavToolbarViewModel), typeof(PathControl), new PropertyMetadata(null));

        public NavToolbarViewModel ViewModel
        {
            get => (NavToolbarViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public PathControl() => InitializeComponent();

        private void BreadCrumbBox_PointerPressed(object sender, PointerRoutedEventArgs e)
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

        private void EditBox_Loaded(object sender, RoutedEventArgs e)
        {
            // AutoSuggestBox won't receive focus unless it's fully loaded
            EditBox.Focus(FocusState.Programmatic);
            DependencyObjectHelpers.FindChild<TextBox>(EditBox)?.SelectAll();
        }

        private void EditBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key is VirtualKey.Escape)
            {
                ViewModel.IsEditModeEnabled = false;
            }
        }

        private void EditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FocusManager.GetFocusedElement() is Control element && element is not AppBarButton)
            {
                if (element.FocusState is not FocusState.Programmatic and not FocusState.Keyboard)
                {
                    ViewModel.IsEditModeEnabled = false;
                }
                else if (ViewModel.IsEditModeEnabled)
                {
                    EditBox.Focus(FocusState.Programmatic);
                }
            }
        }
    }
}
