using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Files.UserControls.Search
{
    public sealed partial class SearchFilterPage : Page
    {
        private static readonly ISearchNavigator navigator = Ioc.Default.GetService<ISearchNavigator>();

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(ISearchPageViewModel), typeof(SearchFilterPage), new PropertyMetadata(null));

        public ISearchPageViewModel ViewModel
        {
            get => (ISearchPageViewModel)GetValue(ViewModelProperty);
            set
            {
                if (ViewModel is not null)
                {
                    ViewModel.Filter.PropertyChanged -= Filter_PropertyChanged;
                }
                SetValue(ViewModelProperty, value);
                if (ViewModel is not null)
                {
                    ViewModel.Filter.PropertyChanged += Filter_PropertyChanged;
                }
            }
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchFilter.IsEmpty))
            {
                ViewModel.Save();
            }
        }

        public SearchFilterPage() => InitializeComponent();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel = e.Parameter as ISearchPageViewModel;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel = null;
        }

        private void BackButton_Tapped(object sender, TappedRoutedEventArgs e) => navigator.Back();
        private void SearchButton_Tapped(object sender, TappedRoutedEventArgs e) => navigator.Search();
        private void ClearButton_Tapped(object sender, TappedRoutedEventArgs e) => ViewModel?.Filter?.Clear();

        private void HeaderCombo_Loaded(object sender, RoutedEventArgs e)
            => (sender as ComboBox).SelectedItem = (ViewModel as IMultiSearchPageViewModel).Filter.Header;
        private void HeaderCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => (ViewModel as IMultiSearchPageViewModel).Key = ((sender as ComboBox).SelectedValue as ISearchHeader).Key;
        private void HeaderCombo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // prevent a bug of lost focus in uwp. This bug close the flyout when combobox is open.
            SearchButton.Focus(FocusState.Programmatic);
        }
    }

    internal class SearchFilterPageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SettingsPageTemplate { get; set; }
        public DataTemplate SinglePageTemplate { get; set; }
        public DataTemplate MultiPageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) => item switch
        {
            ISettingsPageViewModel => SettingsPageTemplate,
            IMultiSearchPageViewModel => MultiPageTemplate,
            ISearchPageViewModel => SinglePageTemplate,
            _ => null,
        };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
            => SelectTemplateCore(item);
    }
}
