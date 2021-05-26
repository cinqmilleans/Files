using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Files.Filesystem.Search
{
    internal interface IFolderSearchOptionProvider
    {
        string Key { get; }
        string Description { get; }

        bool CanProvideOption(object query);
        IFolderSearchOption ProvideOption(object query);

        object[] ProvideSamples();
    }

    internal abstract class FolderSearchOptionProvider : ObservableObject, IFolderSearchOptionProvider
    {
        public string Key { get; protected set; }
        public string Description { get; protected set; }

        public virtual bool CanProvideOption(object query) => true;
        public abstract IFolderSearchOption ProvideOption(object query);

        public virtual object[] ProvideSamples() => new object[0];
    }

    internal class BeforeSearchOptionProvider : FolderSearchOptionProvider
    {
        public BeforeSearchOptionProvider() : base()
        {
            Key = "before";
            Description = "Only before the date";
        }

        public override bool CanProvideOption(object query)
        {
            if (query is DateTime || query is DateTimeOffset)
            {
                return true;
            }
            if (query is string s)
            {
                return DateTime.TryParse(s, out DateTime _);
            }
            return false;
        }

        public override IFolderSearchOption ProvideOption(object query)
        {
            if (query is Moment moment)
            {
                return new BeforeMomentFolderSearchOption(moment);
            }
            if (query is DateTime dateTime)
            {
                return new BeforeDateFolderSearchOption(dateTime);
            }
            if (query is DateTimeOffset dateTimeOffset)
            {
                return ProvideOption(dateTimeOffset.DateTime);
            }
            if (query is string s)
            {
                return ProvideOption(DateTime.Parse(s));
            }

            throw new ArgumentException();
        }

        public override object[] ProvideSamples()
        {
            return new object[]{
                Moment.DayAgo,
                Moment.WeekAgo,
                Moment.MonthAgo,
                Moment.YearAgo,
                Moment.Before,
            };
        }
    }

    internal class AfterSearchOptionProvider : FolderSearchOptionProvider
    {
        public AfterSearchOptionProvider() : base()
        {
            Key = "after";
            Description = "Only after the date";
        }

        public override bool CanProvideOption(object query)
        {
            if (query is DateTime || query is DateTimeOffset)
            {
                return true;
            }
            if (query is string s)
            {
                return DateTime.TryParse(s, out DateTime _);
            }
            return false;
        }

        public override IFolderSearchOption ProvideOption(object query)
        {
            if (query is Moment moment)
            {
                return new AfterMomentFolderSearchOption(moment);
            }
            if (query is DateTime dateTime)
            {
                return new AfterDateFolderSearchOption(dateTime);
            }
            if (query is DateTimeOffset dateTimeOffset)
            {
                return ProvideOption(dateTimeOffset.DateTime);
            }
            if (query is string s)
            {
                return ProvideOption(DateTime.Parse(s));
            }

            throw new ArgumentException();
        }

        public override object[] ProvideSamples()
        {
            return new object[]{
                Moment.DayAgo,
                Moment.WeekAgo,
                Moment.MonthAgo,
                Moment.YearAgo,
                Moment.After,
            };
        }
    }

    internal class AudioFolderSearchFilter : FolderSearchOptionProvider
    {
        public AudioFolderSearchFilter() : base()
        {
            Key = "audio";
            Description = "Only audio files";
        }

        public override IFolderSearchOption ProvideOption(object query) => new AudioFolderSearchOption();
    }
}
