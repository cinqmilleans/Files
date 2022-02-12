using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Files.UserControls.Search
{
    public sealed partial class SearchPane : UserControl
    {
        private ISearchSettings Settings { get; } = Ioc.Default.GetService<ISearchSettings>();
        private ISearchNavigator Navigator { get; } = Ioc.Default.GetService<ISearchNavigator>();

        public SearchPane()
        {
            InitializeComponent();

            Navigator.Initialize(MenuFrame);
            Navigator.GoPage(Settings);
        }

        private void SearchButton_Tapped(object sender, TappedRoutedEventArgs e) => Navigator.Search();


    }
}
