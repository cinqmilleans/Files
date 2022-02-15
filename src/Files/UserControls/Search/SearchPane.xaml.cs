using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class SearchPane : UserControl
    {
        private ISearchNavigator Navigator { get; } = Ioc.Default.GetService<ISearchNavigator>();

        public SearchPane()
        {
            InitializeComponent();

            var settings = Ioc.Default.GetService<ISearchSettings>();

            Navigator.Initialize(MenuFrame);
            Navigator.GoPage(new SearchSettingsViewModel(settings));
        }

        private void SearchButton_Tapped(object sender, TappedRoutedEventArgs e) => Navigator.Search();
    }
}
