using Files.Extensions;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage.Search;

namespace Files.Filesystem.Search
{
   /* internal interface IFolderSearchOption
    {
        void ApplyParameter(QueryOptions query);
    }
    internal interface INamedFolderSearchParameter : IFolderSearchParameter
    {
        string Label { get; }
        string Description { get; }
    }

    internal class FolderSearchParameterCollection : ObservableCollection<IFolderSearchParameter>, IFolderSearchParameter
    {
        public FolderSearchParameterCollection() : base()
        {
        }
        public FolderSearchParameterCollection(IList<IFolderSearchParameter> Parameters) : base(Parameters)
        {
        }

        public void ApplyParameter(QueryOptions query) => Items.ForEach(item => item.ApplyParameter(query));
    }

    internal abstract class FilterFolderSearchParameter : ObservableObject, INamedFolderSearchParameter
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

        public void ApplyParameter(QueryOptions query) => query.UserSearchFilter += $" {Filter}";
    }

    internal class BeforeMomentFolderSearchParameter : FilterFolderSearchParameter
    {
        public BeforeMomentFolderSearchParameter(Moment moment) : base()
        {
            Label = "FolderSearcheParameter.BeforeMoment.Label";//.GetLocalized();
            Description = "FolderSearcheParameter.BeforeMoment.Description";//.GetLocalized();
            Filter = $"date:<={moment.ToAvancedQuerySearch()}";
        }
    }
    internal class BeforeDateFolderSearchParameter : FilterFolderSearchParameter
    {
        public BeforeDateFolderSearchParameter(DateTime dateTime) : base()
        {
            Label = "FolderSearcheParameter.BeforeDate.Label";//.GetLocalized();
            Description = "FolderSearcheParameter.BeforeDate.Description";//.GetLocalized();
            Filter = $"date:<={dateTime}";
        }
    }

    internal class AfterMomentFolderSearchParameter : FilterFolderSearchParameter
    {
        public AfterMomentFolderSearchParameter(Moment moment) : base()
        {
            Label = "FolderSearcheParameter.AfterMoment.Label";//.GetLocalized();
            Description = "FolderSearcheParameter.AfterMoment.Description";//.GetLocalized();
            Filter = $"date:>={moment.ToAvancedQuerySearch()}";
        }
    }
    internal class AfterDateFolderSearchParameter : FilterFolderSearchParameter
    {
        public AfterDateFolderSearchParameter(DateTime dateTime) : base()
        {
            Label = "FolderSearcheParameter.AfterDate.Label";//.GetLocalized();
            Description = "FolderSearcheParameter.AfterDate.Description";//.GetLocalized();
            Filter = $"date:>={dateTime}";
        }
    }

    internal class AudioFolderSearchParameter : FilterFolderSearchParameter
    {
        public AudioFolderSearchParameter()
        {
            Label = "FolderSearcheParameter.Audio.Label";//.GetLocalized();
            Description = "FolderSearcheParameter.Audio.Description";//.GetLocalized();
            Filter = "audio:";
        }
    }*/
}
