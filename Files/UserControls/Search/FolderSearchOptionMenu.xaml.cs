using Files.Filesystem.Search;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Files.UserControls.Search
{
    public sealed partial class FolderSearchOptionMenu : UserControl
    {
        public ObservableCollection<IFolderSearchFilter> BaseFilters { get; }
        public ObservableCollection<IFolderSearchFilter> UserFilters { get; }

        public FolderSearchOptionMenu()
        {
            InitializeComponent();

            var criteria = FolderSearchOption.Default.Filters;
            BaseFilters = new ObservableCollection<IFolderSearchFilter>
            {
                criteria.First(f => f.Key == "creationDate"),
            };
            UserFilters = new ObservableCollection<IFolderSearchFilter>();
        }
    }

    public class FolderSearchFilterTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DateTemplate { get; set; }
        public DataTemplate WhereTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is DateFolderSearchFilter)
            {
                return DateTemplate;
            }
            if (item is StringFolderSearchFilter)
            {
                return WhereTemplate;
            }
            return DateTemplate;
        }
    }
}
