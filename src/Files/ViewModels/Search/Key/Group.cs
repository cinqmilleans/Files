using Files.Enums;
using Files.Extensions;
using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface ISearchFilterViewModelCollection : IReadOnlyCollection<ISearchFilterViewModel>, IMultiSearchFilterViewModel, INotifyCollectionChanged
    {
    }

    [SearchFilterViewModel(SearchKeys.GroupAnd)]
    [SearchFilterViewModel(SearchKeys.GroupOr)]
    [SearchFilterViewModel(SearchKeys.GroupNot)]
    internal class SearchFilterViewModelCollection : ObservableCollection<ISearchFilterViewModel>, ISearchFilterViewModelCollection
    {
        private static readonly ISearchFilterViewModelFactory factory =
            Ioc.Default.GetService<ISearchFilterViewModelFactory>();

        private readonly ISearchFilterCollection filter;

        public ISearchFilter Filter => filter;

        public SearchKeys Key
        {
            get => filter.Key;
            set => filter.Key = value;
        }

        private ISearchHeaderViewModel header;
        public ISearchHeaderViewModel Header => header;

        public bool IsEmpty => filter.IsEmpty;

        private IReadOnlyCollection<ISearchTagViewModel> tags;
        public IEnumerable<ISearchTagViewModel> Tags => tags;

        private readonly RelayCommand clearCommand;
        public ICommand ClearCommand => clearCommand;

        public SearchFilterViewModelCollection(ISearchFilterCollection filter)
        {
            this.filter = filter;
            clearCommand = new RelayCommand((filter as ISearchFilter).Clear, () => !filter.IsEmpty);

            filter.ForEach(f => Add(factory.GetFilterViewModel(f)));

            UpdateHeader();
            UpdateTags();

            filter.PropertyChanged += Filter_PropertyChanged;
            filter.CollectionChanged += Filter_CollectionChanged;
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IMultiSearchFilter.Key):
                    OnPropertyChanged(nameof(Key));
                    break;
                case nameof(ISearchFilter.Header):
                    UpdateHeader();
                    OnPropertyChanged(nameof(Header));
                    break;
                case nameof(ISearchFilter.IsEmpty):
                    OnPropertyChanged(nameof(IsEmpty));
                    clearCommand.NotifyCanExecuteChanged();
                    break;
                case nameof(ISearchFilter.Tags):
                    UpdateTags();
                    OnPropertyChanged(nameof(Tags));
                    break;
            }
        }
        private void Filter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void UpdateHeader()
            => header = new SearchHeaderViewModel(filter.Header);
        private void UpdateTags()
            => tags = filter.Tags.Select(tag => new SearchTagViewModel(this, tag)).ToList().AsReadOnly();

        private void OnPropertyChanged(string propertyName)
            => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    [SearchPageViewModel(SearchKeys.GroupAnd)]
    [SearchPageViewModel(SearchKeys.GroupOr)]
    [SearchPageViewModel(SearchKeys.GroupNot)]
    internal class GroupPageViewModel : MultiSearchPageViewModel
    {
        public GroupPageViewModel(ISearchPageViewModel parent, ISearchFilterViewModelCollection filter)
            : base(parent, filter) {}

        protected override IEnumerable<SearchKeys> GetKeys() => new List<SearchKeys>
        {
            SearchKeys.GroupAnd,
            SearchKeys.GroupOr,
            SearchKeys.GroupNot,
        };
    }
}
