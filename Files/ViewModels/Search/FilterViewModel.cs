using Files.Extensions;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IFilterViewModel
    {
        IFilter Filter { get; }

        ICommand ClearCommand { get; }
        ICommand SelectCommand { get; }
        ICommand BackCommand { get; }
    }

    public interface IFilterViewModelFactory
    {
        IFilterViewModel GetViewModel(IFilter filter);
    }

    public class FilterViewModelFactory : IFilterViewModelFactory
    {
        private readonly INavigatorViewModel navigator;

        public FilterViewModelFactory(INavigatorViewModel navigator) => this.navigator = navigator;

        public IFilterViewModel GetViewModel(IFilter filter) => filter switch
        {
            IFilterCollection f => new FilterViewModelCollection(navigator, f),
            IOperatorFilter f => new OperatorViewModel(navigator, f),
            IDateRangeFilter f => new DateRangeViewModel(navigator, f),
            ISizeRangeFilter f => new SizeRangeViewModel(navigator, f),
            _ => null,
        };
    }

    public abstract class FilterViewModel<T> : ObservableObject, IFilterViewModel where T : IFilter
    {
        IFilter IFilterViewModel.Filter => Filter;
        public T Filter { get; }

        public ICommand ClearCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand BackCommand { get; }

        protected INavigatorViewModel Navigator { get; }

        public FilterViewModel(INavigatorViewModel navigator, T filter)
        {
            Navigator = navigator;
            Filter = filter;
            ClearCommand = new RelayCommand(() => Filter.Clear());
            SelectCommand = new RelayCommand(() => Navigator.OpenPage(this));
            BackCommand = navigator.BackCommand;
        }
    }

    public class FilterViewModelCollection : FilterViewModel<IFilterCollection>
    {
        private readonly IFilterViewModelFactory factory;

        private IList<IFilterViewModel> subFilters = new List<IFilterViewModel>();
        public IEnumerable<IFilterViewModel> SubFilters => subFilters;

        public FilterViewModelCollection(INavigatorViewModel navigator, IFilterCollection filter) : base(navigator, filter)
        {
            factory = new FilterViewModelFactory(navigator);
            filter.ForEach(filter => Add(filter));
        }

        public void Add(IFilter filter)
        {
            subFilters.Add(factory.GetViewModel(filter));
            OnPropertyChanged(nameof(SubFilters));
        }
        public void Remove(IFilter filter)
        {
                subFilters.Remove(factory.GetViewModel(filter));
            OnPropertyChanged(nameof(SubFilters));
        }
    }

    public class OperatorViewModel : FilterViewModel<IOperatorFilter>
    {
        private readonly IFilterViewModelFactory factory;

        private IFilterViewModel subFilter;
        public IFilterViewModel SubFilter => subFilter;

        public OperatorViewModel(INavigatorViewModel navigator, IOperatorFilter filter) : base(navigator, filter)
        {
            factory = new FilterViewModelFactory(navigator);
            Set(filter.SubFilter);
        }

        public void Unset() => Set(null);
        public void Set(IFilter filter)
        {
            subFilter = factory.GetViewModel(filter);
            OnPropertyChanged(nameof(Filter));
        }
    }
}
