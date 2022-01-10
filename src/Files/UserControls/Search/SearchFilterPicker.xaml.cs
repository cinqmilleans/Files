using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPicker : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchFilter), typeof(SearchFilterPicker), new PropertyMetadata(null));

        public ISearchFilter ViewModel
        {
            get => (ISearchFilter)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public SearchFilterPicker() => InitializeComponent();

        private void OpenButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var navigator = Ioc.Default.GetService<ISearchNavigator>();
            var filter = (sender as FrameworkElement).DataContext as ISearchFilter;
            navigator.GoPage(filter);
        }

        private void AddFilterButton_Loaded(object sender, RoutedEventArgs e)
        {
            var menu = new MenuFlyout
            {
                Placement = FlyoutPlacementMode.BottomEdgeAlignedRight
            };

            var file = new MenuFlyoutSubItem { Text = "File".GetLocalized() };
            file.Items.Add(GetItem(new SizeHeader()));
            file.Items.Add(new MenuFlyoutSeparator());
            file.Items.Add(GetItem(new CreatedHeader()));
            file.Items.Add(GetItem(new ModifiedHeader()));
            file.Items.Add(GetItem(new AccessedHeader()));
            menu.Items.Add(file);

            var group = new MenuFlyoutSubItem { Text = "Group".GetLocalized() };
            group.Items.Add(GetItem(new AndHeader()));
            group.Items.Add(GetItem(new OrHeader()));
            group.Items.Add(GetItem(new NotHeader()));
            menu.Items.Add(group);

            (sender as Button).Flyout = menu;

            MenuFlyoutItem GetItem(ISearchHeader header) => new()
            {
                Tag = header,
                Template = HeaderItemTemplate,
            };
        }

        private void AddItemButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var navigator = Ioc.Default.GetService<ISearchNavigator>();
            var header = (sender as Button).Content as ISearchHeader;
            var filter = header.GetFilter();
            navigator.GoPage(filter);
        }
    }

    public class SearchFilterPickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GroupTemplate { get; set; }
        public DataTemplate SizeRangeTemplate { get; set; }
        public DataTemplate DateRangeTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISearchFilterCollection => GroupTemplate,
            ISizeRangeFilter => SizeRangeTemplate,
            IDateRangeFilter => DateRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
