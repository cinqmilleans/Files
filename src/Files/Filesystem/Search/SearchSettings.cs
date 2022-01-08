﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Files.Filesystem.Search
{
    public interface ISearchSettings : INotifyPropertyChanged
    {
        bool SearchInSubFolders { get; set; }

        ISearchFilterCollection Filter { get; }
    }

    public class SearchSettings : ObservableObject, ISearchSettings
    {
        private readonly IReadOnlyDictionary<string, ISearchHeader> Headers
            = GetHeaders().ToDictionary(header => header.Key);

        public bool searchInSubFolders = true;
        public bool SearchInSubFolders
        {
            get => searchInSubFolders;
            set => SetProperty(ref searchInSubFolders, value);
        }

        public ISearchFilterCollection Filter { get; } = new SearchFilterCollection(GroupAggregates.And);

        public SearchSettings()
        {
            var pinnedKeys = new string[] { "size", "modified" };

            var filters = pinnedKeys.Select(key => Headers[key].GetFilter()).ToList();
            Filter = new SearchFilterCollection(GroupAggregates.And, filters);
        }

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
