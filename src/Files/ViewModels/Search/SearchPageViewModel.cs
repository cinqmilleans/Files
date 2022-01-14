using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Files.ViewModels.Search
{
    public interface ISearchPageViewModel : INotifyPropertyChanged
    {
        ISearchPageViewModel Parent { get; }

        ISearchFilter Filter { get; }
    }

    public interface ISettingsPageViewModel : ISearchPageViewModel
    {
        ISearchSettings Settings { get; }
    }

    public interface IMultiSearchPageViewModel : ISearchPageViewModel
    {
        SearchKeys Key { get; set; }
        IEnumerable<ISearchHeader> Headers { get; }
    }

    public interface ISearchPageViewModelFactory
    {
        ISearchPageViewModel GetPageViewModel(ISearchPageViewModel parent, ISearchFilter filter);
    }

    public class SearchPageViewModel : ObservableObject, ISearchPageViewModel
    {
        public ISearchPageViewModel Parent { get; }

        public ISearchFilter Filter { get; }

        public SearchPageViewModel(ISearchPageViewModel parent, ISearchFilter filter)
        {
            (Parent, Filter) = (parent, filter);

            Filter.PropertyChanged += Filter_PropertyChanged;
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Filter.IsEmpty))
            {
                this.Save();
            }
        }
    }

    public class SettingsPageViewModel : SearchPageViewModel, ISettingsPageViewModel
    {
        public ISearchSettings Settings { get; }

        public SettingsPageViewModel(ISearchSettings settings)
            : base(null, settings.Filter) => Settings = settings;
    }

    public abstract class MultiSearchPageViewModel : SearchPageViewModel, IMultiSearchPageViewModel
    {
        public SearchKeys Key
        {
            get => Filter.Header.Key;
            set
            {
                if (Filter.Header.Key != value)
                {
                    (Filter as IMultiSearchFilter).Key = value;
                    OnPropertyChanged(nameof(Key));
                }
            }
        }

        public IEnumerable<ISearchHeader> Headers { get; }

        public MultiSearchPageViewModel(ISearchPageViewModel parent, IMultiSearchFilter filter) : base(parent, filter)
        {
            var provider = Ioc.Default.GetService<ISearchHeaderProvider>();
            Headers = GetKeys().Select(key => provider.GetHeader(key)).ToList();
        }

        protected abstract IEnumerable<SearchKeys> GetKeys();
    }

    public class SearchPageViewModelFactory : ISearchPageViewModelFactory
    {
        public ISearchPageViewModel GetPageViewModel(ISearchPageViewModel parent, ISearchFilter filter) => filter switch
        {
            ISearchSettings s => new SettingsPageViewModel(s),
            ISearchFilterCollection f => new GroupPageViewModel(parent, f),
            IDateRangeFilter f => new DateRangePageViewModel(parent, f),
            ISearchFilter f => new SearchPageViewModel(parent, f),
            _ => null,
        };
    }

    public static class SearchPageViewModelExtensions
    {
        public static void Save (this ISearchPageViewModel pageViewModel)
        {
            var collection = pageViewModel?.Parent?.Filter as ISearchFilterCollection;
            if (collection is null)
            {
                return;
            }
            var filter = pageViewModel.Filter;

            var settings = Ioc.Default.GetService<ISearchSettings>();
            bool isPinned = pageViewModel.Parent is ISettingsPageViewModel && collection.IndexOf(filter) < settings.PinnedKeys.Count;

            if (!filter.IsEmpty)
            {
                if (!collection.Contains(filter))
                {
                    collection.Add(filter);
                }
            }
            else if (!isPinned)
            {
                collection.Remove(filter);
            }
        }
    }
}
