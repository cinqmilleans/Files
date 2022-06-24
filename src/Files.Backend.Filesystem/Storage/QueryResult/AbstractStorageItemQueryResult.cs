﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public abstract class AbstractStorageItemQueryResult<T> : IStorageItemQueryResult where T : IStorageItem
    {
        private static readonly Regex spaceSplitRegex = new("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        public IBaseStorageFolder Folder { get; }
        public QueryOptions Options { get; }

        public AbstractStorageItemQueryResult(IBaseStorageFolder folder, QueryOptions options) => (Folder, Options) = (folder, options);

        protected static IAsyncOperation<IImmutableList<T>> ToResult(Task<IEnumerable<T>> items)
            => AsyncInfo.Run<IImmutableList<T>>(async (_) => (await items).ToImmutableList());

        protected IEnumerable<T> Select(IEnumerable<T> items)
        {
            string query = string.Join(" ", Options.ApplicationSearchFilter, Options.UserSearchFilter).Trim();
            if (!string.IsNullOrEmpty(query))
            {
                var spaceSplit = spaceSplitRegex.Split(query);
                foreach (var split in spaceSplit)
                {
                    var colonSplit = split.Split(':');
                    if (colonSplit.Length is 2)
                    {
                        if (colonSplit[0] is "name" or "fileName" or "System.FileName")
                        {
                            string pattern = CleanPattern(colonSplit[1]);
                            items = items.Where(x => Regex.IsMatch(x.Name, pattern));
                        }
                    }
                    else
                    {
                        string pattern = CleanPattern(split);
                        items = items.Where(x => Regex.IsMatch(x.Name, pattern));
                    }
                }
            }
            return items;

            static string CleanPattern(string pattern) => pattern
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("*", "(.*?)", StringComparison.Ordinal);
        }
    }
}
