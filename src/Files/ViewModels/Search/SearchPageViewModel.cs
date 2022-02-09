using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchPageViewModel : INotifyPropertyChanged
    {
        ISearchPageViewModel Parent { get; }

        ISearchContent Content { get; }
        ISearchFilter Filter { get; }

        ICommand ClearCommand { get; }
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SearchPageViewModelAttribute : Attribute
    {
        public SearchKeys Key { get; set; } = SearchKeys.None;

        public SearchPageViewModelAttribute() {}
        public SearchPageViewModelAttribute(SearchKeys key) => Key = key;
    }

    public class SearchPageViewModel : ObservableObject, ISearchPageViewModel
    {
        public ISearchPageViewModel Parent { get; }

        public ISearchContent Content => Filter;
        public ISearchFilter Filter { get; }

        public ICommand ClearCommand { get; }

        public SearchPageViewModel(ISearchPageViewModel parent, ISearchFilter filter)
        {
            (Parent, Filter) = (parent, filter);
            ClearCommand = new RelayCommand(Filter.Clear);
        }
    }

    public class SettingsPageViewModel : ObservableObject, ISettingsPageViewModel
    {
        public ISearchPageViewModel Parent => null;

        public ISearchSettings Settings { get; }

        public ISearchContent Content => Settings;
        public ISearchFilter Filter => Settings.Filter;

        public ICommand ClearCommand { get; }

        public SettingsPageViewModel(ISearchSettings settings)
        {
            Settings = settings;
            ClearCommand = new RelayCommand(Settings.Clear);
        }
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
        private readonly IReadOnlyDictionary<SearchKeys, Factory> factories = GetFactories();

        public ISearchPageViewModel GetPageViewModel(ISearchPageViewModel parent, ISearchFilter filter) => filter switch
        {
            ISearchSettings s => new SettingsPageViewModel(s),
            ISearchFilter f when factories.ContainsKey(f.Header.Key) => factories[f.Header.Key].Build(parent, f),
            ISearchFilter f => new SearchPageViewModel(parent, f),
            _ => null,
        };

        private static IReadOnlyDictionary<SearchKeys, Factory> GetFactories()
        {
            var factories = new Dictionary<SearchKeys, Factory>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(SearchPageViewModelAttribute), false).Cast<SearchPageViewModelAttribute>();
                foreach (var attribute in attributes)
                {
                    factories[attribute.Key] = new Factory(type);
                }
            }

            return new ReadOnlyDictionary<SearchKeys, Factory>(factories);
        }

        private class Factory
        {
            private readonly Type type;

            public Factory(Type type) => this.type = type;

            public ISearchPageViewModel Build(ISearchPageViewModel parent, ISearchFilter filter)
                => Activator.CreateInstance(type, new object[] { parent, filter }) as ISearchPageViewModel;
        }
    }

    public static class SearchPageViewModelExtensions
    {
        public static void Save (this ISearchPageViewModel pageViewModel)
        {
            var filter = pageViewModel?.Filter;
            if (!filter.IsEmpty)
            {
                var collection = pageViewModel?.Parent?.Filter as ISearchFilterCollection;
                if (collection is not null && !collection.Contains(filter))
                {
                    collection.Add(filter);
                    pageViewModel?.Parent?.Save();
                }
            }
        }
    }
}
