using Files.Extensions;
using Files.Filesystem.Search;
using Files.UserControls.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchFilterViewModel : INotifyPropertyChanged
    {
        ICommand ClearCommand { get; }

        ISearchContext Context { get; }
        ISearchFilter Filter { get; }
        IEnumerable<ISearchHeader> Alternatives { get; }
    }

    public interface ISearchFilterViewModelFactory
    {
        ISearchFilterViewModel GetViewModel(ISearchFilter filter);
    }

    public interface ISearchContext
    {
        ICommand SearchCommand { get; }
        ICommand BackCommand { get; }

        void Search();
        void Save(ISearchFilter filter);

        void Back();
        void GoPage(ISearchFilter filter);

        ISearchContext GetChild(ISearchFilter filter);
    }

    public class SearchFilterViewModel<T> : ObservableObject, ISearchFilterViewModel where T : ISearchFilter
    {
        public ISearchContext Context { get; }

        ISearchFilter ISearchFilterViewModel.Filter => Filter;
        public T Filter { get; }

        public virtual IEnumerable<ISearchHeader> Alternatives => Filter.Header.CreateEnumerable();

        public ICommand ClearCommand { get; }

        public SearchFilterViewModel(ISearchContext context, T filter)
        {
            Context = context;
            Filter = filter;

            ClearCommand = new RelayCommand(Filter.Clear);
        }
    }

    public class SearchFilterViewModelFactory : ISearchFilterViewModelFactory
    {
        private readonly ISearchContext context;

        public SearchFilterViewModelFactory(ISearchContext context) => this.context = context;

        public ISearchFilterViewModel GetViewModel(ISearchFilter filter) => filter switch
        {
            //ISearchFilterCollection f => new GroupPageViewModel(context, f),
            //IDateRangeFilter f => new DateRangePageViewModel(context, f),
            ISizeRangeFilter f => new SearchFilterViewModel<ISizeRangeFilter>(context, f),
            _ => null,
        };
    }

    public class SearchContext : ISearchContext
    {
        private readonly ISearchNavigator navigator;

        private readonly ISearchFilterCollection collection;
        private ISearchFilter filter;

        public ICommand SearchCommand { get; }
        public ICommand BackCommand { get; }

        private SearchContext()
        {
            SearchCommand = new RelayCommand(Search);
            BackCommand = new RelayCommand(Back);
        }
        public SearchContext(ISearchNavigator navigator, ISearchFilter filter)
            : this() => (this.navigator, this.filter) = (navigator, filter);
        private SearchContext(ISearchNavigator navigator, ISearchFilterCollection collection, ISearchFilter filter)
            : this(navigator, filter) => this.collection = collection;

        public void Search() => navigator?.Search();
        public void Back() => navigator?.Back();

        public void GoPage(ISearchFilter filter)
        {
            var child = GetChild(filter);
            var factory = new SearchFilterViewModelFactory(child);
            var viewModel = factory.GetViewModel(filter);

            navigator.GoPage(viewModel);
        }

        public void Save(ISearchFilter filter)
        {
            if (collection is null)
            {
                return;
            }
            if (filter is null && collection.Contains(this.filter))
            {
                collection.Remove(this.filter);
            }
            else if (filter is not null)
            {
                if (collection.Contains(this.filter))
                {
                    int index = collection.IndexOf(this.filter);
                    collection[index] = filter;
                }
                else
                {
                    collection.Add(filter);
                }
            }
            this.filter = filter;
        }

        public ISearchContext GetChild(ISearchFilter filter)
            => new SearchContext(navigator, this.filter as ISearchFilterCollection, filter);
    }
}
