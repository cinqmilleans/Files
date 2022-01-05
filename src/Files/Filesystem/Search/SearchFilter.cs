using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Files.Filesystem.Search
{
    public interface ISearchHeader
    {
        string Key { get; }
        string Glyph { get; }
        string Title { get; }
        string Description { get; }

        ISearchFilter GetFilter();
    }

    public interface ISearchFilter : INotifyPropertyChanged
    {
        ISearchHeader Header { get; }
        IEnumerable<ISearchTag> Tags { get; }

        string ToAdvancedQuerySyntax();
    }

    public interface ISearchTag
    {
        ISearchFilter Filter { get; }

        string Title { get; }
        string Parameter { get; }

        void Delete();
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SearchHeaderAttribute : Attribute {}
}
