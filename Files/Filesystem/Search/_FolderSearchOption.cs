using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files.Filesystem.Search
{
   /*internal interface IFolderSearchParameter
    {
        string Key { get; }
        string Description { get; }

        bool CanProvideOption(object query);
        IFolderSearchParameter ProvideOption(object query);

        object[] ProvideSamples();
    }

    internal abstract class _FolderSearchFilter : ObservableObject, IFolderSearchFilter
    {
        public string Key { get; protected set; }
        public string Description { get; protected set; }

        public virtual bool CanProvideOption(object query) => true;
        public abstract IFolderSearchOption ProvideOption(object query);

        public virtual object[] ProvideSamples() => new object[0];
    }

    internal class BeforeFolderSearchFilter : _FolderSearchFilter
    {
        public BeforeFolderSearchFilter() : base()
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
                return new BeforeFolderSearchOption(moment);
            }
            if (query is DateTime dateTime)
            {
                return new BeforeFolderSearchOption(dateTime);
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
                Moment.Custom,
            };
        }
    }

    internal class AfterFolderSearchFilter : _FolderSearchFilter
    {
        public AfterFolderSearchFilter() : base()
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
                return new AfterFolderSearchOption(moment);
            }
            if (query is DateTime dateTime)
            {
                return new AfterFolderSearchOption(dateTime);
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
                Moment.Custom,
            };
        }
    }

    internal class AudioFolderSearchFilter : _FolderSearchFilter
    {
        public AudioFolderSearchFilter() : base()
        {
            Key = "audio";
            Description = "Only audio files";
        }

        public override IFolderSearchOption ProvideOption(object query) => new AudioFolderSearchOption();
    }*/
}
