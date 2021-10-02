using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IFilterPageViewModel
    {
        INavigatorViewModel Navigator { get; }

        IContainerFilterViewModel Parent { get; }
        IFilterViewModel Filter { get; }

        //ICommand ClearCommand { get; }
        //ICommand OpenPageCommand { get; }
        ICommand BackCommand { get; }
        ICommand SaveCommand { get; }
        //ICommand BackSaveCommand { get; }

        //void Clear();
        //void OpenPage();
        //void Back();
        //void Save();
    }

    public class FilterPageViewModel : IFilterPageViewModel
    {
        public INavigatorViewModel Navigator { get; set; }

        public IContainerFilterViewModel Parent { get; set; }
        public IFilterViewModel Filter { get; set; }

        //public ICommand ClearCommand { get; }
        //public ICommand OpenPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }
        //public ICommand BackSaveCommand { get; }

        public FilterPageViewModel()
        {
            SaveCommand = new RelayCommand(Save);
        }

        public void Back() => Navigator?.Back();

        public void Save()
        {
            var filter = Filter?.Filter;
            var parent = Parent?.Filter;

            if (filter is not null && parent is not null)
            {
                if (filter.IsEmpty)
                {
                    parent.Unset(filter);
                }
                else if (!filter.IsEmpty)
                {
                    parent.Set(filter);
                }
            }

            Navigator?.Back();
        }
    }
}
