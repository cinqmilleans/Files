using Files.Filesystem.Search;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class TagControl : UserControl
    {
        public static readonly DependencyProperty SearchTagProperty =
            DependencyProperty.Register(nameof(SearchTag), typeof(ISearchTag), typeof(TagControl), new PropertyMetadata(null));

        public ISearchTag SearchTag
        {
            get => (ISearchTag)GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        public TagControl() => InitializeComponent();

        private void MainButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var button = element.FindDescendant("CloseButton") as Button;
                if (button is not null)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }
        private void MainButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var button = element.FindDescendant("CloseButton") as Button;
                if (button is not null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void MainButton_Tapped(object sender, TappedRoutedEventArgs e) {}
        private void CloseButton_Tapped(object sender, TappedRoutedEventArgs e) => SearchTag.Delete();
    }
}
