using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    internal class BaseStorageFolderQueryResult
    {
        public BaseStorageFolder Folder { get; }
        public QueryOptions Options { get; }

        public BaseStorageFolderQueryResult(BaseStorageFolder folder, QueryOptions options)
        {
            Folder = folder;
            Options = options;
        }

        public virtual IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = (await GetFoldersAsync()).Skip((int)startIndex).Take((int)Math.Min(maxNumberOfItems, int.MaxValue));
                return items.ToList();
            });
        }

        public virtual IAsyncOperation<IReadOnlyList<BaseStorageFolder>> GetFoldersAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFolder>>(async (cancellationToken) =>
            {
                var items = await Folder.GetFoldersAsync();
                var query = string.Join(" ", Options.ApplicationSearchFilter, Options.UserSearchFilter).Trim();
                if (!string.IsNullOrEmpty(query))
                {
                    var spaceSplit = Regex.Split(query, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    foreach (var split in spaceSplit)
                    {
                        var colonSplit = split.Split(':');
                        if (colonSplit.Length == 2)
                        {
                            if (colonSplit[0] == "System.FileName" || colonSplit[0] == "fileName" || colonSplit[0] == "name")
                            {
                                items = items.Where(x => Regex.IsMatch(x.Name, colonSplit[1].Replace("\"", "", StringComparison.Ordinal).Replace("*", "(.*?)", StringComparison.Ordinal), RegexOptions.IgnoreCase)).ToList();
                            }
                        }
                        else
                        {
                            items = items.Where(x => Regex.IsMatch(x.Name, split.Replace("\"", "", StringComparison.Ordinal).Replace("*", "(.*?)", StringComparison.Ordinal), RegexOptions.IgnoreCase)).ToList();
                        }
                    }
                }
                return items.ToList();
            });
        }

        public virtual StorageFolderQueryResult ToStorageFolderQueryResult() => null;
    }

}
