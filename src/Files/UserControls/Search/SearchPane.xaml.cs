using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class SearchPane : UserControl
    {
        private readonly ISearchSettings settings = Ioc.Default.GetService<ISearchSettings>();
        private readonly ISearchNavigator navigator = Ioc.Default.GetService<ISearchNavigator>();

        public SearchPane()
        {
            InitializeComponent();

            //navigator.Initialize(null, MenuFrame);
            //navigator.GoPage(settings);
        }
    }
}
