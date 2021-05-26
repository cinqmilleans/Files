using Files.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage.Search;

namespace Files.Filesystem.Search
{
    internal interface IFolderSearchOption
    {
        void ApplyOption(QueryOptions query);
    }
    internal interface IUserFolderSearchOption : IFolderSearchOption
    {
        string Label { get; }
        string Description { get; }
    }

    internal class FolderSearchOptionCollection : ObservableCollection<IFolderSearchOption>, IFolderSearchOption
    {
        public string Label => string.Empty;
        public string Description { get; }


        public FolderSearchOptionCollection() : base()
        {
        }
        public FolderSearchOptionCollection(IList<IFolderSearchOption> options) : base(options)
        {
        }

        public void ApplyOption(QueryOptions query) => Items.ForEach(item => item.ApplyOption(query));
    }

    internal abstract class FilterFolderSearchOption : ObservableObject, IUserFolderSearchOption
    {
        private string label;
        public string Label
        {
            get => label;
            protected set => SetProperty(ref label, value);
        }

        private string description;
        public string Description
        {
            get => description;
            protected set => SetProperty(ref description, value);
        }

        protected string Filter { get; set; }

        public void ApplyOption(QueryOptions query) => query.UserSearchFilter += $" {Filter}";
    }

    internal class BeforeMomentFolderSearchOption : FilterFolderSearchOption
    {
        public BeforeMomentFolderSearchOption(Moment moment) : base()
        {
            Label = "FolderSearcheOption.BeforeMoment.Label";//.GetLocalized();
            Description = "FolderSearcheOption.BeforeMoment.Description";//.GetLocalized();
            Filter = $"date:<={moment.ToAdvancedSearchQueryFilter()}";
        }
    }
    internal class BeforeDateFolderSearchOption : FilterFolderSearchOption
    {
        public BeforeDateFolderSearchOption(DateTime dateTime) : base()
        {
            Label = "FolderSearcheOption.BeforeDate.Label";//.GetLocalized();
            Description = "FolderSearcheOption.BeforeDate.Description";//.GetLocalized();
            Filter = $"date:<={dateTime}";
        }
    }

    internal class AfterMomentFolderSearchOption : FilterFolderSearchOption
    {
        public AfterMomentFolderSearchOption(Moment moment) : base()
        {
            Label = "FolderSearcheOption.AfterMoment.Label";//.GetLocalized();
            Description = "FolderSearcheOption.AfterMoment.Description";//.GetLocalized();
            Filter = $"date:>={moment.ToAdvancedSearchQueryFilter()}";
        }
    }
    internal class AfterDateFolderSearchOption : FilterFolderSearchOption
    {
        public AfterDateFolderSearchOption(DateTime dateTime) : base()
        {
            Label = "FolderSearcheOption.AfterDate.Label";//.GetLocalized();
            Description = "FolderSearcheOption.AfterDate.Description";//.GetLocalized();
            Filter = $"date:>={dateTime}";
        }
    }

    internal class AudioFolderSearchOption : FilterFolderSearchOption
    {
        public AudioFolderSearchOption()
        {
            Label = "FolderSearcheOption.Audio.Label";//.GetLocalized();
            Description = "FolderSearcheOption.Audio.Description";//.GetLocalized();
            Filter = "audio:";
        }
    }
}
