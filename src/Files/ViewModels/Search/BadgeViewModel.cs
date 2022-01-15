using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Files.ViewModels.Search
{
    public interface IBadgeViewModel
    {
        int Count { get; }
    }

    public class BadgeViewModel : ObservableObject, IBadgeViewModel
    {
        private readonly ISearchFilterCollection collection;

        public int Count => collection.Count(filter => !filter.IsEmpty);

        public BadgeViewModel(ISearchFilterCollection collection)
        {
            this.collection = collection;
            collection.CollectionChanged += Collection_CollectionChanged;

            foreach (var filter in collection)
            {
                filter.PropertyChanged += Filter_PropertyChanged;
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action is NotifyCollectionChangedAction.Remove)
            {
                foreach (ISearchFilter filter in e.OldItems)
                {
                    filter.PropertyChanged -= Filter_PropertyChanged;
                }
            }
            if (e.Action is NotifyCollectionChangedAction.Add)
            {
                foreach (ISearchFilter filter in e.NewItems)
                {
                    filter.PropertyChanged += Filter_PropertyChanged;
                }
            }

            OnPropertyChanged(nameof(Count));
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ISearchFilter.IsEmpty))
            {
                OnPropertyChanged(nameof(Count));
            }
        }
    }
}
