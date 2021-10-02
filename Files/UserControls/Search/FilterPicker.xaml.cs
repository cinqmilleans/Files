﻿using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Files.UserControls.Search
{
    public sealed partial class FilterPicker : UserControl
    {
        private readonly IFilterViewModelFactory factory = new FilterViewModelFactory();

        //public static readonly DependencyProperty NavigatorProperty =
        //    DependencyProperty.Register(nameof(Navigator), typeof(INavigatorViewModel), typeof(FilterPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(IFilterPageViewModel), typeof(FilterPicker), new PropertyMetadata(null));

        //public INavigatorViewModel Navigator
        //{
        //    get => (INavigatorViewModel)GetValue(NavigatorProperty);
        //    set => SetValue(NavigatorProperty, value);
        //}

        public IFilterPageViewModel ViewModel
        {
            get => (IFilterPageViewModel)GetValue(ViewModelProperty);
            set
            {
                SetValue(ViewModelProperty, value);

                //var factory = new FilterViewModelFactory();
                //MainControl.Content = factory.GetViewModel(value);
            }
        }

        public FilterPicker() => InitializeComponent();

        private MenuFlyout GetMenu()
        {
            var menu = new MenuFlyout{ Placement = FlyoutPlacementMode.BottomEdgeAlignedRight };

            var file = new MenuFlyoutSubItem { Text = "File" };
            file.Items.Add(GetMenuItem(new CreatedFilter()));
            file.Items.Add(GetMenuItem(new ModifiedFilter()));
            file.Items.Add(GetMenuItem(new FileSizeFilter()));
            menu.Items.Add(file);

            var image = new MenuFlyoutSubItem { Text = "Image" };
            image.Items.Add(GetMenuItem("Aspect Ratio"));
            image.Items.Add(GetMenuItem("Resolution"));
            image.Items.Add(GetMenuItem("Format"));
            menu.Items.Add(image);

            var video = new MenuFlyoutSubItem { Text = "Video" };
            video.Items.Add(GetMenuItem("Aspect Ratio"));
            video.Items.Add(GetMenuItem("Resolution"));
            video.Items.Add(GetMenuItem("Format"));
            menu.Items.Add(video);

            var op = new MenuFlyoutSubItem { Text = "Operator" };
            op.Items.Add(GetMenuItem(new AndFilter()));
            op.Items.Add(GetMenuItem(new OrFilter()));
            op.Items.Add(GetMenuItem(new NotFilter()));
            menu.Items.Add(op);

            return menu;
        }

        private MenuFlyoutItem GetMenuItem(IFilter filter) => new()
        {
            Icon = new FontIcon { FontSize = 14, Glyph = filter.Glyph },
            Text = filter.ShortLabel,
            Command = new RelayCommand(() => Select(filter)),
        };
        private static MenuFlyoutItem GetMenuItem(string text) => new() { Text = text };

        private void Select(IFilter filter)
        {
            //Navigator?.OpenPage(filter);
            var factory = new FilterViewModelFactory();
            var viewModel = new FilterPageViewModel
            {
                Navigator = ViewModel.Navigator,
                Parent = ViewModel.Filter as IContainerFilterViewModel,
                Filter = factory.GetViewModel(filter),
            };
            ViewModel.Navigator.OpenPage(viewModel);
        }

        private void AddFilter_Loaded(object sender, RoutedEventArgs e)
            => (sender as Button).Flyout = GetMenu();

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }

    public class FilterPickerTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate OperatorTemplate { get; set; }
        public DataTemplate DateRangeTemplate { get; set; }
        public DataTemplate SizeRangeTemplate { get; set; }
        public DataTemplate KindTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            IFilterCollectionViewModel => CollectionTemplate,
            IOperatorFilterViewModel => OperatorTemplate,
            IDateRangeFilterViewModel _ => DateRangeTemplate,
            ISizeRangeFilterViewModel _ => SizeRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }

    public class FilterButtonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            IFilterCollection => DefaultTemplate,
            IOperatorFilter => DefaultTemplate,
            IDateRangeFilter => DefaultTemplate,
            ISizeRangeFilter => DefaultTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }

    public class FilterButtonContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate OperatorTemplate { get; set; }
        public DataTemplate DateRangeTemplate { get; set; }
        public DataTemplate SizeRangeTemplate { get; set; }
        public DataTemplate KindTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            IFilterCollection => CollectionTemplate,
            IOperatorFilter => OperatorTemplate,
            IDateRangeFilter => DateRangeTemplate,
            ISizeRangeFilter => SizeRangeTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
