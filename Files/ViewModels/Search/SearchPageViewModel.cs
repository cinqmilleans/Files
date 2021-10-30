﻿using Files.Filesystem.Search;
using Files.UserControls.Search;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchPageViewModel : INotifyPropertyChanged
    {
        ISearchFilterHeader Header { get; }
        IPickerViewModel Picker { get; }

        ICommand BackCommand { get; }
        ICommand SaveCommand { get; }
        ICommand AcceptCommand { get; }
    }

    public interface IMultiSearchPageViewModel : ISearchPageViewModel
    {
        IEnumerable<ISearchFilterHeader> Headers { get; }
        new ISearchFilterHeader Header { get; set; }
    }

    public interface ISearchPageContext
    {
        void Back();
        void GoPage(ISearchFilter filter);
        void Save(ISearchFilter filter);
    }

    public interface ISearchFilterHeader
    {
        string Glyph { get; }
        string Title { get; }
        string Description { get; }

        ISearchFilter GetFilter();
    }

    public interface ISearchFilterContext
    {
        string Glyph { get; }
        string Label { get; }
        string Parameter { get; }

        ICommand ClearCommand { get; }
        ICommand OpenCommand { get; }

        ISearchFilter GetFilter();
    }

    public interface IPickerViewModel : INotifyPropertyChanged
    {
        bool IsEmpty { get; }
        ICommand ClearCommand { get; }
    }

    public interface ISearchPageViewModelFactory
    {
        ISearchPageViewModel GetViewModel(ISearchFilter filter);
    }

    public interface ISearchFilterContextFactory
    {
        ISearchFilterContext GetContext(ISearchFilter filter);
    }

    public class SearchFilterHeader<T> : ISearchFilterHeader where T : ISearchFilter, IHeader, new()
    {
        public string Glyph { get; }
        public string Title { get; }
        public string Description { get; }

        public SearchFilterHeader()
        {
            var filter = new T();
            Glyph = filter.Glyph;
            Title = filter.Title;
            Description = filter.Description;
        }

        ISearchFilter ISearchFilterHeader.GetFilter() => GetFilter();
        public T GetFilter() => new();
    }

    public class SearchPageViewModelFactory : ISearchPageViewModelFactory
    {
        private readonly ISearchPageContext context;

        public SearchPageViewModelFactory(ISearchPageContext context) => this.context = context;

        public ISearchPageViewModel GetViewModel(ISearchFilter filter) => filter switch
        {
            //ISearchFilterCollection f => new GroupPageViewModel(context, f),
            //IDateRangeFilter f => new DateRangePageViewModel(context, f),
            //ISizeRangeFilter f => new SizeRangePageViewModel(context, f),
            _ => null,
        };
    }

    public class SearchFilterContextFactory : ISearchFilterContextFactory
    {
        private readonly ISearchPageContext parent;

        public SearchFilterContextFactory(ISearchPageContext parent) => this.parent = parent;

        public ISearchFilterContext GetContext(ISearchFilter filter) => filter switch
        {
            //ISearchFilterCollection f => new GroupContext(parent, f),
            //IDateRangeFilter f => new DateRangeContext(parent, f),
            //ISizeRangeFilter f => new SizeRangeContext(parent, f),
            _ => null,
        };
    }

    public class SearchPageContext : ISearchPageContext
    {
        private readonly ISearchNavigator navigator;

        private readonly ISearchFilterCollection collection;
        private readonly ISearchFilter filter;

        public SearchPageContext(ISearchNavigator navigator, ISearchFilter filter)
            => (this.navigator, this.filter) = (navigator, filter);
        private SearchPageContext(ISearchNavigator navigator, ISearchFilterCollection collection, ISearchFilter filter) : this(navigator, filter)
            => this.collection = collection;

        public void Back() => navigator.GoBack();

        public void GoPage(ISearchFilter filter)
        {
            var child = new SearchPageContext(navigator, this.filter as ISearchFilterCollection, filter);
            var factory = new SearchPageViewModelFactory(child);
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
        }
    }
}
