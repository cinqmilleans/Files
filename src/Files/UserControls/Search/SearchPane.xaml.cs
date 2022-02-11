using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class SearchPane : UserControl
    {
        public SearchPane()
        {
            InitializeComponent();

            var settings = Ioc.Default.GetService<ISearchSettings>();
            var navigator = Ioc.Default.GetService<ISearchNavigator>();

            navigator.Initialize(null, MenuFrame);
            navigator.GoPage(settings);
        }
    }
}
