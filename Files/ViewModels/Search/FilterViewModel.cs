﻿using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace Files.ViewModels.Search
{
    public interface IFilterViewModelFactory
    {
        IFilterViewModel GetViewModel(IFilter filter);
    }

    public interface IFilterViewModel : INotifyPropertyChanged
    {
        IFilter Filter { get; }
    }
    public interface IFilterViewModel<T> : IFilterViewModel where T : IFilter
    {
        new T Filter { get; }
    }

    public interface IFilterCollectionViewModel : IFilterViewModel<IFilterCollection>
    {
    }
    public interface IOperatorFilterViewModel : IFilterViewModel<IOperatorFilter>
    {
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

    public class FilterViewModel<T> : ObservableObject, IFilterViewModel<T> where T : IFilter
    {
        IFilter IFilterViewModel.Filter => Filter;
        public T Filter { get; }

        public FilterViewModel(T filter) => Filter = filter;
    }

    public class FilterCollectionViewModel : FilterViewModel<IFilterCollection>, IFilterCollectionViewModel
    {
        public FilterCollectionViewModel(IFilterCollection filter) : base(filter) {}
    }
    public class OperatorFilterViewModel : FilterViewModel<IOperatorFilter>, IOperatorFilterViewModel
    {
        public OperatorFilterViewModel(IOperatorFilter filter) : base(filter) { }
    }
}
