using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
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
            IDateRangeFilter f => new DateRangeViewModel(f),
            ISizeRangeFilter f => new SizeRangeViewModel(f),
            _ => null,
        };
    }

    public class FilterViewModel<T> : ObservableObject, IFilterViewModel where T : IFilter
    {
        IFilter IFilterViewModel.Filter => Filter;
        public T Filter { get; }

        public ICommand ClearCommand { get; }

        public FilterViewModel(T filter)
        {
            Filter = filter;
            ClearCommand = new RelayCommand(Clear);
        }

        private void Clear() => Filter?.Clear();
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
