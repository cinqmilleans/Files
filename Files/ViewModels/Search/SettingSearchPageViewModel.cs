using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISettingSearchPageViewModel : INotifyPropertyChanged
    {
        string Glyph { get; }
        string Title { get; }

        bool HasValue { get; }

        ICommand BackCommand { get; }
        ICommand SelectCommand { get; }
    }

    public abstract class SettingSearchPageViewModel : ObservableObject, ISettingSearchPageViewModel
    {
        public abstract string Glyph { get; }
        public abstract string Title { get; }

        public abstract bool HasValue { get; }

        public ICommand BackCommand => Navigator.BackCommand;
        public ICommand SelectCommand { get; }

        protected ISearchNavigatorViewModel Navigator { get; }

        public SettingSearchPageViewModel(ISearchNavigatorViewModel navigator)
        {
            Navigator = navigator;
            SelectCommand = new RelayCommand(() => Navigator.OpenPage(this));
        }
    }
}
