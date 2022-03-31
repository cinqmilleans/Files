using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    internal class BaseStorageItemQueryResult : IStorageItemQueryResult
    {
        private static readonly Regex splitRegex = new("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        public IBaseStorageFolder Folder { get; }
        public QueryOptions Options { get; }

        public BaseStorageItemQueryResult(BaseStorageFolder folder, QueryOptions options)
            => (Folder, Options) = (folder, options);

        public virtual IStorageItemQueryResult? ToStorageItemQueryResult() => null;

        public virtual IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = (await GetItemsAsync()).Skip((int)startIndex).Take((int)Math.Min(maxNumberOfItems, int.MaxValue));
                return items.ToList();
            });
        }

        public virtual IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await Folder.GetItemsAsync();
                var query = string.Join(" ", Options.ApplicationSearchFilter, Options.UserSearchFilter).Trim();
                if (!string.IsNullOrEmpty(query))
                {
                    var spaceSplit = splitRegex.Split(query);
                    foreach (var split in spaceSplit)
                    {
                        var colonSplit = split.Split(':');
                        if (colonSplit.Length is 2)
                        {
                            if (colonSplit[0] is "name" or "fileName" or "System.FileName")
                            {
                                return items.Where(x => Regex.IsMatch(x.Name, Clean(colonSplit[1]), RegexOptions.IgnoreCase)).ToList();
                            }
                        }
                        else
                        {
                            return items.Where(x => Regex.IsMatch(x.Name, Clean(split), RegexOptions.IgnoreCase)).ToList();
                        }
                    }
                }
                return items.ToList();
            });
        }

        private static string Clean(string item) => item.Replace("\"", string.Empty).Replace("*", "(.*?)");
    }
}
