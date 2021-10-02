using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IFilterViewModelFactory
    {
        IFilterViewModel GetViewModel(IFilter filter);
    }

    public interface IFilterViewModel : INotifyPropertyChanged
    {
        IFilter Filter { get; }
        ICommand ClearCommand { get; }
    }
    public interface IContainerFilterViewModel : IFilterViewModel
    {
        new IContainerFilter Filter { get; }
    }
    public interface IFilterCollectionViewModel : IContainerFilterViewModel
    {
        new IFilterCollection Filter { get; }
    }
    public interface IOperatorFilterViewModel : IContainerFilterViewModel
    {
        new IOperatorFilter Filter { get; }
    }

    public class FilterViewModelFactory : IFilterViewModelFactory
    {
        public IFilterViewModel GetViewModel(IFilter filter) => filter switch
        {
            IFilterCollection f => new FilterCollectionViewModel(f),
            IOperatorFilter f => new OperatorFilterViewModel(f),
            IDateRangeFilter f => new DateRangeFilterViewModel(f),
            ISizeRangeFilter f => new SizeRangeFilterViewModel(f),
            _ => null,
        };
    }

    public class FilterViewModel<T> : ObservableObject, IFilterViewModel where T : IFilter
    {
        public IContainerFilterViewModel Parent { get; }

        IFilter IFilterViewModel.Filter => Filter;
        public T Filter { get; }

        public ICommand ClearCommand { get; }
        public ICommand OpenPageCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand BackSaveCommand { get; }

        public FilterViewModel(T filter) : this(null, filter)
        {
        }
        public FilterViewModel(IContainerFilterViewModel parent, T filter)
        {
            Parent = parent;
            Filter = filter;

            ClearCommand = new RelayCommand(Clear);
            OpenPageCommand = new RelayCommand(OpenPage);
            BackCommand = new RelayCommand(Back);
            SaveCommand = new RelayCommand(Save);
            BackSaveCommand = new RelayCommand(BackSave);
        }

        private void Clear() => Filter?.Clear();
        public void OpenPage()
        {
            throw new System.NotImplementedException();
        }

        public void Back()
        {
            throw new System.NotImplementedException();
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        private void BackSave()
        {
            Back();
            Save();
        }
    }

    public class FilterCollectionViewModel : FilterViewModel<IFilterCollection>, IFilterCollectionViewModel
    {
        IContainerFilter IContainerFilterViewModel.Filter => Filter;

        public FilterCollectionViewModel(IFilterCollection filter): base(filter) {}
    }
    public class OperatorFilterViewModel : FilterViewModel<IOperatorFilter>, IOperatorFilterViewModel
    {
        IContainerFilter IContainerFilterViewModel.Filter => Filter;

        public OperatorFilterViewModel(IOperatorFilter filter) : base(filter) {}
    }
}
