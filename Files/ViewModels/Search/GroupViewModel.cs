﻿using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Files.ViewModels.Search
{
    public interface IGroupPageViewModel : IMultiSearchPageViewModel
    {
        new IGroupPickerViewModel Picker { get; }
    }

    public interface IGroupPickerViewModel : IPickerViewModel
    {
        string Description { get; set; }
        ISearchFilterCollection Filters { get; }
        IEnumerable<ISearchFilterContext> Contexts { get; }

        ICommand OpenCommand { get; }
    }

    public interface IGroupHeader : ISearchFilterHeader
    {
        ISearchFilterCollection GetFilter(IEnumerable<ISearchFilter> filters);
    }

    public interface IGroupContext : ISearchFilterContext
    {
    }

    public class AndHeader : SearchFilterHeader<AndFilterCollection>, IGroupHeader
    {
        ISearchFilterCollection IGroupHeader.GetFilter(IEnumerable<ISearchFilter> filters) => GetFilter(filters);
        public AndFilterCollection GetFilter(IEnumerable<ISearchFilter> filters) => new(filters);
    }
    public class OrHeader : SearchFilterHeader<OrFilterCollection>, IGroupHeader
    {
        ISearchFilterCollection IGroupHeader.GetFilter(IEnumerable<ISearchFilter> filters) => GetFilter(filters);
        public OrFilterCollection GetFilter(IEnumerable<ISearchFilter> filters) => new(filters);
    }
    public class NotHeader : SearchFilterHeader<NotFilterCollection>, IGroupHeader
    {
        ISearchFilterCollection IGroupHeader.GetFilter(IEnumerable<ISearchFilter> filters) => GetFilter(filters);
        public NotFilterCollection GetFilter(IEnumerable<ISearchFilter> filters) => new(filters);
    }

    public class GroupContext : IGroupContext
    {
        private readonly ISearchPageContext context;
        private readonly ISearchFilterCollection filter;

        public string Glyph => filter.Glyph;
        public string Label => filter.Title;
        public string Parameter => filter.Count switch
        {
            <= 1 => string.Format("SearchGroupHeader_ItemSuffixe".GetLocalized(), filter.Count),
            _ => string.Format("SearchGroupHeader_ItemsSuffixe".GetLocalized(), filter.Count),
        };

        public ICommand ClearCommand { get; }
        public ICommand OpenCommand { get; }

        public GroupContext(ISearchPageContext context, ISearchFilterCollection filter)
        {
            this.context = context;
            this.filter = filter;

            ClearCommand = new RelayCommand(Clear);
            OpenCommand = new RelayCommand(Open);
        }

        ISearchFilter ISearchFilterContext.GetFilter() => filter;
        public ISearchFilterCollection GetFilter() => filter;

        private void Clear() => context.Save(null);
        private void Open() => context.GoPage(filter);
    }

    public class GroupPageViewModel : ObservableObject, IGroupPageViewModel
    {
        private readonly ISearchPageContext context;

        public IEnumerable<ISearchFilterHeader> Headers { get; } = new List<ISearchFilterHeader>
        {
            new AndHeader(),
            new OrHeader(),
            new NotHeader(),
        };

        private ISearchFilterHeader header;
        public ISearchFilterHeader Header
        {
            get => header;
            set
            {
                if (SetProperty(ref header, value))
                {
                    Picker.Description = header.Description;
                }
            }
        }

        IPickerViewModel ISearchPageViewModel.Picker => Picker;
        public IGroupPickerViewModel Picker { get; }

        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AcceptCommand { get; }

        public GroupPageViewModel(ISearchPageContext context) : this(context, null)
        {
        }
        public GroupPageViewModel(ISearchPageContext context, ISearchFilterCollection filter)
        {
            this.context = context;

            filter ??= new AndFilterCollection();

            header = filter switch
            {
                AndFilterCollection => Headers.First(h => h is AndHeader),
                OrFilterCollection => Headers.First(h => h is OrHeader),
                NotFilterCollection => Headers.First(h => h is NotHeader),
                _ => Headers.First(),
            };

            Picker = new GroupPickerViewModel(context, filter);
            Picker.Description = header?.Description;

            BackCommand = new RelayCommand(Back);
            SaveCommand = new RelayCommand(Save);
            AcceptCommand = new RelayCommand(Accept);
        }

        public void Back() => context.Back();
        public void Save()
        {
            if (Picker.IsEmpty)
            {
                context.Save(null);
            }
            else
            {
                var header = Header as IGroupHeader;
                var filter = header.GetFilter(Picker.Filters);
                context.Save(filter);
            }
        }
        public void Accept()
        {
            Save();
            Back();
        }
    }

    public class GroupPickerViewModel : ObservableObject, IGroupPickerViewModel
    {
        private readonly ISearchPageContext context;

        public bool IsEmpty => !Filters.Any();

        private string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public ISearchFilterCollection Filters { get; }

        public IEnumerable<ISearchFilterContext> Contexts
        {
            get
            {
                var factory = new SearchFilterContextFactory(context);
                return Filters.Select(filter => factory.GetContext(filter));
            }
        }

        public ICommand ClearCommand { get; }
        public ICommand OpenCommand { get; }

        public GroupPickerViewModel(ISearchPageContext context, ISearchFilterCollection filters) : base()
        {
            this.context = context;

            Filters = filters;
            Filters.PropertyChanged += Filters_PropertyChanged;

            ClearCommand = new RelayCommand(Clear);
            OpenCommand = new RelayCommand<ISearchFilterHeader>(Open);
        }

        public void Clear() => Filters.Clear();

        private void Open(ISearchFilterHeader header)
        {
            var filter = header.GetFilter();
            context.GoPage(filter);
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(Contexts));
        }

    }
}
