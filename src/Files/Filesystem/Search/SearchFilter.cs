using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Files.Filesystem.Search
{
    public interface ISearchHeaderManager
    {
        bool HasHeader(string key);
        ISearchHeader GetHeader(string key);
    }

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

    public class SearchHeaderManager : ISearchHeaderManager
    {
        private readonly IReadOnlyDictionary<string, ISearchHeader> headers = GetHeaders().ToDictionary(header => header.Key);

        public bool HasHeader(string key) => headers.ContainsKey(key);
        public ISearchHeader GetHeader(string key) => headers[key];

        private static IEnumerable<ISearchHeader> GetHeaders()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(SearchHeaderAttribute), false);
                if (attributes.Length == 1)
                {
                    var header = Activator.CreateInstance(type) as ISearchHeader;
                    if (header is not null)
                    {
                        yield return header;
                    }
                }
            }
        }
    }
}
