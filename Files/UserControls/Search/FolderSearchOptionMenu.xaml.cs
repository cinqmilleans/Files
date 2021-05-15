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

        private void PeriodComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as ComboBox;
            var items = new ComboBoxItem[]
            {
                new ComboBoxItem{ Content = " ", Tag = DateFolderSearchFilter.Periods.None },
                new ComboBoxItem{ Content = "A day ago", Tag = DateFolderSearchFilter.Periods.DayAgo },
                new ComboBoxItem{ Content = "A week ago", Tag = DateFolderSearchFilter.Periods.WeekAgo },
                new ComboBoxItem{ Content = "A month ago", Tag = DateFolderSearchFilter.Periods.MonthAgo },
                new ComboBoxItem{ Content = "A year ago", Tag = DateFolderSearchFilter.Periods.YearAgo },
                new ComboBoxItem{ Content = "Custom", Tag = DateFolderSearchFilter.Periods.Custom },
            };
            control.ItemsSource = items;
            control.SelectedItem = items[0];
        }

        private void PeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as Control;
            var filter = control.DataContext as DateFolderSearchFilter;
            var period = (DateFolderSearchFilter.Periods)control.Tag;
            filter.Period = period;
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
