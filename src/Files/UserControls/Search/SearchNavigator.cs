using Files.Extensions;
using Files.Filesystem.Search;
using Files.ViewModels.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Files.UserControls.Search
{
    public interface ISearchNavigator
    {
        void Initialize(ISearchBox box, Frame frame);

        void Search();
        void Back();
        void Save();

        void ClearPage();
        void GoPage(ISearchSettings settings);
        void GoPage(ISearchFilter filter);

    }

    public class SearchNavigator : ISearchNavigator
    {
        private readonly ISearchSettings settings = Ioc.Default.GetService<ISearchSettings>();

        private readonly Stack<ISearchFilter> filterStack = new();

        private readonly NavigationTransitionInfo emptyTransition =
            new SuppressNavigationTransitionInfo();
        private readonly NavigationTransitionInfo toRightTransition =
            new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };

        private ISearchBox box;
        private Frame frame;

        public void Initialize(ISearchBox box, Frame frame) => (this.box, this.frame) = (box, frame);

        public void Search()
        {
            if (box is not null)
            {
                if (string.IsNullOrWhiteSpace(box.Query))
                {
                    box.Query = "*";
                }
                //SearchBox.Search();
                //SearchBox.IsMenuOpen = false;
            }
        }

        public void Back()
        {
            if (frame is not null && frame.CanGoBack)
            {
                filterStack.Pop();
                frame.GoBack(toRightTransition);
            }
        }

        public void Save()
        {
            var filters = filterStack.CloneStack();

            filters.TryPop(out ISearchFilter filter);
            var collection = PopCollection(filters);

            while (collection is not null)
            {
                if (!filter.IsEmpty)
                {
                    if (!collection.Contains(filter))
                    {
                        collection.Add(filter);
                    }
                }
                else if (!settings.PinnedFilters.Contains(filter))
                {
                    collection.Remove(filter);
                }

                filter = collection;
                collection = PopCollection(filters);
            }

            static ISearchFilterCollection PopCollection(Stack<ISearchFilter> filters)
            {
                filters.TryPop(out ISearchFilter filter);
                return filter as ISearchFilterCollection;
            }
        }

        public void ClearPage()
        {
            if (frame is not null)
            {
                filterStack.Clear();
                frame.Content = null;
            }
        }
        public void GoPage(ISearchSettings settings)
        {
            var viewModel = new SettingsPageViewModel(settings);
            filterStack.Clear();
            filterStack.Push(viewModel.Filter);
            frame?.Navigate(typeof(SearchFilterPage), viewModel, emptyTransition);
        }
        public void GoPage(ISearchFilter filter)
        {
            ISearchPageViewModel viewModel = filter switch
            {
                IDateRangeFilter f => new DateRangePageViewModel(f),
                ISearchFilter f => new SearchPageViewModel(f),
                _  => null,
            };

            if (viewModel is not null)
            {
                filterStack.Push(filter);
                frame?.Navigate(typeof(SearchFilterPage), viewModel, toRightTransition);
            }
        }
    }
}
