using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Files.Filesystem.Search
{
    public interface ISearchFilterManager
    {
        ISearchFilter GetFilter(string key);
        string GetKey(ISearchFilter filter);
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SearchFilterAttribute : Attribute
    {
        public string Key { get; }

        public SearchFilterAttribute(string key) => Key = key;
    }

    public class SearchFilterManager : ISearchFilterManager
    {
        private readonly IDictionary<string, Type> types = GetTypes();

        public ISearchFilter GetFilter(string key) => Activator.CreateInstance(types[key]) as ISearchFilter;
        public string GetKey(ISearchFilter filter) => types.First(type => type.Value == filter.GetType()).Key;

        private static IDictionary<string, Type> GetTypes()
        {
            var types = new Dictionary<string, Type>();

            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes(typeof(SearchFilterAttribute), false);
                if (attributes.Length == 1)
                {
                    var attribute = attributes[0] as SearchFilterAttribute;
                    types.Add(attribute.Key, type);
                }
            }

            return types;
        }
    }
}
