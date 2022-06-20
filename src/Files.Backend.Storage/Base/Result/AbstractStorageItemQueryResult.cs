using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Storage
{
    public abstract class AbstractStorageItemQueryResult : IStorageItemQueryResult
    {
        private static readonly Regex spaceSplitRegex = new("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        public IBaseStorageFolder Folder { get; }
        public QueryOptions Options { get; }

        public AbstractStorageItemQueryResult(IBaseStorageFolder folder, QueryOptions options) => (Folder, Options) = (folder, options);

        protected IAsyncOperation<IImmutableList<T>> GetFilteredItems<T>(Task<IEnumerable<T>> getItemAsync) where T : IStorageItem
        {
            return AsyncInfo.Run(async (CancellationToken _) =>
            {
                var items = await getItemAsync.AsAsyncOperation();
                return (IImmutableList<T>)FilterItems(items).ToImmutableList();
            });
        }
        protected IAsyncOperation<IImmutableList<T>> GetFilteredItems<T>
            (Task<IEnumerable<T>> getItemAsync, uint startIndex, uint maxNumberOfItems) where T : IStorageItem
        {
            int skip = (int)startIndex;
            int take = (int)Math.Min(maxNumberOfItems, int.MaxValue);

            return AsyncInfo.Run(async (CancellationToken _) =>
            {
                var items = await getItemAsync.AsAsyncOperation();
                return (IImmutableList<T>)FilterItems(items).Skip(skip).Take(take).ToImmutableList();
            });
        }

        private IEnumerable<T> FilterItems<T>(IEnumerable<T> items) where T : IStorageItem
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
        }

        private static string CleanPattern(string pattern) => pattern
            .Replace("\"", string.Empty, StringComparison.Ordinal)
            .Replace("*", "(.*?)", StringComparison.Ordinal);
    }
}
