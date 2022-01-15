using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Files.Filesystem.Search
{
    public enum SearchKeys : ushort
    {
        None,
        GroupAnd,
        GroupOr,
        GroupNot,
        Size,
        DateCreated,
        DateModified,
        DateAccessed,
    }

    public interface ISearchHeader
    {
        SearchKeys Key { get; }

        string Glyph { get; }
        string Label { get; }
        string Description { get; }

        ISearchFilter CreateFilter();
    }

    public interface ISearchFilter : ISearchContent
    {
        ISearchHeader Header { get; }

        IEnumerable<ISearchTag> Tags { get; }

        string ToAdvancedQuerySyntax();
    }

    public interface IMultiSearchFilter : ISearchFilter
    {
        SearchKeys Key { get; set; }
    }

    public interface ISearchTag
    {
        ISearchFilter Filter { get; }

        string Title { get; }
        string Parameter { get; }

        void Delete();
    }

    public interface ISearchContent : INotifyPropertyChanged
    {
        bool IsEmpty { get; }
        void Clear();
    }

    public interface ISearchHeaderProvider
    {
        ISearchHeader GetHeader(SearchKeys key);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SearchHeaderAttribute : Attribute
    {
        public SearchKeys Key { get; set; } = SearchKeys.None;

        public SearchHeaderAttribute() {}
        public SearchHeaderAttribute(SearchKeys key) => Key = key;
    }

    public class SearchHeaderProvider : ISearchHeaderProvider
    {
        private readonly IReadOnlyDictionary<SearchKeys, ISearchHeader> headers
            = new ReadOnlyDictionary<SearchKeys, ISearchHeader>(GetHeaders().ToDictionary(header => header.Key));

        public ISearchHeader GetHeader(SearchKeys key) => headers?[key];

        private static IEnumerable<ISearchHeader> GetHeaders()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(SearchHeaderAttribute), false).Cast<SearchHeaderAttribute>();
                foreach (var attribute in attributes)
                {
                    yield return attribute.Key is SearchKeys.None
                        ? Activator.CreateInstance(type) as ISearchHeader
                        : Activator.CreateInstance(type, new object[] { attribute.Key }) as ISearchHeader;
                }
            }
        }
    }
}
