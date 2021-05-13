using Files.Filesystem.Search;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class SearchSettingMenu : UserControl
    {
        public FolderSearchOption Option { get; } = new FolderSearchOption();

        public SearchSettingMenu()
        {
            InitializeComponent();
        }
    }
}
