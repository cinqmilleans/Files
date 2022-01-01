﻿using Files.Filesystem.Search;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
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
        bool HasDescription { get; }
        string Description { get; set; }
        ISearchFilterCollection Filters { get; }
        IEnumerable<ISearchFilterContext> Contexts { get; }

        ICommand OpenCommand { get; }

        IEnumerable<object> Tests { get; }
    }

    public class Test
    {
        public bool IsChecked { get; set; } = true;
        public string Title { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
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

    public class GroupContext : SearchFilterContext<ISearchFilterCollection>, IGroupContext
    {
        public override string Parameter
        {
            get
            {
                int count = GetFilter().Count;
                return count switch
                {
                    <= 1 => string.Format("SearchGroupHeader_ItemSuffixe".GetLocalized(), count),
                    _ => string.Format("SearchGroupHeader_ItemsSuffixe".GetLocalized(), count),
                };
            }
        }

        public GroupContext(ISearchPageContext parentPageContext, ISearchFilterCollection filter) : base(parentPageContext, filter) {}
    }

    public class GroupPageViewModel : ObservableObject, IGroupPageViewModel
    {
        private readonly ISearchPageContext context;

        ISearchPageNavigator ISearchPageViewModel.Navigator => context;

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
                    Save();
                }
            }
        }

        IPickerViewModel ISearchPageViewModel.Picker => Picker;
        public IGroupPickerViewModel Picker { get; }

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

            Picker = new GroupPickerViewModel(context, filter, Save)
            {
                Description = header?.Description
            };
        }

        private void Save() => context.Save(!Picker.IsEmpty ? Picker.Filters : null);
    }

    public class GroupPickerViewModel : ObservableObject, IGroupPickerViewModel
    {
        public IEnumerable<object> Tests => new object[]
        {
            new Test{ IsChecked = false, Title = "Modification Date" },
            new Test{ IsChecked = false, Title = "Size" },
            new Test{ Title = "Creation Date:", Value = " 2021" },
            "Kind:",
            new Test{ Value = " PDF" },
            new Test{ Value = "Docx" },
            "Access Date:",
            new Test{ Title = "after:", Value = " 01/08/2021" },
            new Test{ Title = "before:", Value = " 01/14/2021" },
        };


        private readonly ISearchPageContext context;
        private readonly Action saveAction;

        public bool IsEmpty => !Filters.Any();
        public bool HasDescription => !string.IsNullOrEmpty(Description);

        private string description;
        public string Description
        {
            get => description;
            set
            {
                if (SetProperty(ref description, value))
                {
                    OnPropertyChanged(nameof(HasDescription));
                }
            }
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

        public GroupPickerViewModel(ISearchPageContext context, ISearchFilterCollection filters, Action saveAction)
        {
            this.context = context;
            this.saveAction = saveAction;

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
            saveAction();
        }
    }
}
