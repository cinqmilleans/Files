using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Storage.Search;

namespace Files.Backend.Filesystem.Storage
{
    public class BaseStorageFileQueryResult
    {
        public BaseStorageFolder Folder { get; }
        public QueryOptions Options { get; }

        public BaseStorageFileQueryResult(BaseStorageFolder folder, QueryOptions options)
        {
            Folder = folder;
            Options = options;
        }

        public virtual IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = (await GetFilesAsync()).Skip((int)startIndex).Take((int)Math.Min(maxNumberOfItems, int.MaxValue));
                return items.ToList();
            });
        }

        public virtual IAsyncOperation<IReadOnlyList<BaseStorageFile>> GetFilesAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<BaseStorageFile>>(async (cancellationToken) =>
            {
                var items = await Folder.GetFilesAsync();
                var query = string.Join(" ", Options.ApplicationSearchFilter, Options.UserSearchFilter).Trim();
                if (!string.IsNullOrEmpty(query))
                {
                    var spaceSplit = Regex.Split(query, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                    foreach (var split in spaceSplit)
                    {
                        var colonSplit = split.Split(":");
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

        public virtual StorageFileQueryResult ToStorageFileQueryResult() => null;

    }
}
