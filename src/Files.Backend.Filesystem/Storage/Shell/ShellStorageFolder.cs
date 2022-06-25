using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Shared;
using Files.Shared.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using S = Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public class ShellStorageFolder : BaseStorageFolder
    {
        private static readonly IFullTrustAsker asker = Ioc.Default.GetService<IFullTrustAsker>();

        public override string Path { get; }
        public override string Name { get; }
        public override string DisplayName => Name;
        public override string DisplayType { get; }
        public override string FolderRelativeId => $"0\\{Name}";

        public override DateTimeOffset DateCreated { get; }
        public override S.FileAttributes Attributes => S.FileAttributes.Directory;
        public override IStorageItemExtraProperties Properties => new BaseBasicStorageItemExtraProperties(this);

        public ShellStorageFolder(ShellFileItem item)
        {
            Name = item.FileName;
            Path = item.RecyclePath; // True path on disk
            DateCreated = item.CreatedDate;
            DisplayType = item.FileType;
        }

        public static bool IsShellPath(string path)
        {
            return path is not null &&
                path.StartsWith("shell:", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("::{", StringComparison.Ordinal) ||
                path.StartsWith(@"\\SHELL\", StringComparison.Ordinal);
        }

        public static ShellStorageFolder FromShellItem(ShellFileItem item)
        {
            if (item is ShellLinkItem linkItem)
            {
                return new ShortcutStorageFolder(linkItem);
            }
            else if (item.RecyclePath.Contains("$Recycle.Bin", StringComparison.Ordinal))
            {
                return new BinStorageFolder(item);
            }
            else
            {
                return new ShellStorageFolder(item);
            }
        }

        public static IAsyncOperation<BaseStorageFolder> FromPathAsync(string path)
        {
            return AsyncInfo.Run<BaseStorageFolder>(async (cancellationToken) =>
            {
                if (IsShellPath(path))
                {
                    var res = await GetFolderAndItems(path, false);
                    if (res.Folder != null)
                    {
                        return FromShellItem(res.Folder);
                    }
                }
                return null;
            });
        }

        protected static async Task<(ShellFileItem Folder, List<ShellFileItem> Items)> GetFolderAndItems(string path, bool enumerate, int startIndex = 0, int maxItemsToRetrieve = int.MaxValue)
        {
            if (asker is not null)
            {
                var parameter = new ValueSet
                {
                    ["Arguments"] = "ShellFolder",
                    ["action"] = enumerate ? "Enumerate" : "Query",
                    ["from"] = startIndex,
                    ["count"] = maxItemsToRetrieve,
                    ["folder"] = path,
                };

                var response = await asker.GetResponseAsync(parameter);
                if (response.IsSuccess)
                {
                    var folder = JsonConvert.DeserializeObject<ShellFileItem>(response.Get("Folder", string.Empty));
                    var items = JsonConvert.DeserializeObject<List<ShellFileItem>>(response.Get("Enumerate", string.Empty), new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        SerializationBinder = new KnownTypesBinder() { KnownTypes = { typeof(ShellFileItem), typeof(ShellLinkItem) } }
                    });
                    return (folder, items);
                }
            }
            return (null, null);
        }

        public override IAsyncOperation<StorageFolder> ToStorageFolderAsync() => throw new NotSupportedException();

        public override bool IsEqual(IStorageItem item) => item?.Path == Path;
        public override bool IsOfType(StorageItemTypes type) => type == StorageItemTypes.Folder;

        public override IAsyncOperation<IndexedState> GetIndexedStateAsync() => Task.FromResult(IndexedState.NotIndexed).AsAsyncOperation();

        public override IAsyncOperation<IBaseStorageFolder> GetParentAsync() => throw new NotSupportedException();

        public override IAsyncOperation<IBaseBasicProperties> GetBasicPropertiesAsync()
        {
            return AsyncInfo.Run<IBaseBasicProperties>(async (cancellationToken) =>
            {
                var res = await GetFolderAndItems(Path, false);
                if (res.Folder is not null)
                {
                    return new ShellBasicProperties(res.Folder);
                }
                return new BaseBasicProperties();
            });
        }

        public override IAsyncOperation<IStorageItem> GetItemAsync(string name)
        {
            return AsyncInfo.Run<IStorageItem>(async (cancellationToken) =>
            {
                var res = await GetFolderAndItems(Path, true);
                if (res.Items is null)
                {
                    return null;
                }

                var entry = res.Items.FirstOrDefault(x => x.FileName != null && x.FileName.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (entry is null)
                {
                    return null;
                }

                if (entry.IsFolder)
                {
                    return ShellStorageFolder.FromShellItem(entry);
                }

                return ShellStorageFile.FromShellItem(entry);
            });
        }
        public override IAsyncOperation<IStorageItem> TryGetItemAsync(string name)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                try
                {
                    return await GetItemAsync(name);
                }
                catch
                {
                    return null;
                }
            });
        }
        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
            => AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken)
                => (await GetItemsAsync(0, int.MaxValue)).ToList()
            );
        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxItemsToRetrieve)
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var res = await GetFolderAndItems(Path, true, (int)startIndex, (int)maxItemsToRetrieve);
                if (res.Items is null)
                {
                    return null;
                }

                var items = new List<IStorageItem>();
                foreach (var entry in res.Items)
                {
                    if (entry.IsFolder)
                    {
                        items.Add(FromShellItem(entry));
                    }
                    else
                    {
                        items.Add(ShellStorageFile.FromShellItem(entry));
                    }
                }
                return items;
            });
        }

        public override IAsyncOperation<IBaseStorageFile> GetFileAsync(string name)
            => AsyncInfo.Run<IBaseStorageFile>(async (cancellationToken) => await GetItemAsync(name) as ShellStorageFile);
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync()
            => AsyncInfo.Run<IEnumerable<IBaseStorageFile>>(async (cancellationToken) => (await GetItemsAsync())?.OfType<ShellStorageFile>());
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query)
            => AsyncInfo.Run(async (cancellationToken) => await GetFilesAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFile>> GetFilesAsync(CommonFileQuery query, uint startIndex, uint maxItemsToRetrieve)
            => AsyncInfo.Run(async (cancellationToken)
                => (await GetFilesAsync()).Skip((int)startIndex).Take((int)maxItemsToRetrieve)
            );

        public override IAsyncOperation<IBaseStorageFolder> GetFolderAsync(string name)
            => AsyncInfo.Run<IBaseStorageFolder>(async (cancellationToken) => await GetItemAsync(name) as ShellStorageFolder);
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync()
            => AsyncInfo.Run<IEnumerable<IBaseStorageFolder>>(async (cancellationToken) => (await GetItemsAsync())?.OfType<ShellStorageFolder>());
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query)
            => AsyncInfo.Run(async (cancellationToken) => await GetFoldersAsync());
        public override IAsyncOperation<IEnumerable<IBaseStorageFolder>> GetFoldersAsync(CommonFolderQuery query, uint startIndex, uint maxItemsToRetrieve)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                var items = await GetFoldersAsync();
                return items.Skip((int)startIndex).Take((int)maxItemsToRetrieve);
            });
        }

        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName) => throw new NotSupportedException();
        public override IAsyncOperation<IBaseStorageFile> CreateFileAsync(string desiredName, CreationCollisionOption options)
            => throw new NotSupportedException();

        public override IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName) => throw new NotSupportedException();
        public override IAsyncOperation<IBaseStorageFolder> CreateFolderAsync(string desiredName, CreationCollisionOption options)
            => throw new NotSupportedException();

        public override IAsyncAction RenameAsync(string desiredName) => throw new NotSupportedException();
        public override IAsyncAction RenameAsync(string desiredName, NameCollisionOption option) => throw new NotSupportedException();

        public override IAsyncAction DeleteAsync() => throw new NotSupportedException();
        public override IAsyncAction DeleteAsync(StorageDeleteOption option) => throw new NotSupportedException();

        public override bool AreQueryOptionsSupported(QueryOptions queryOptions) => false;
        public override bool IsCommonFileQuerySupported(CommonFileQuery query) => false;
        public override bool IsCommonFolderQuerySupported(CommonFolderQuery query) => false;

        public override StorageItemQueryResult CreateItemQuery() => throw new NotSupportedException();
        public override IBaseStorageItemQueryResult CreateItemQueryWithOptions(QueryOptions queryOptions) => new BaseStorageItemQueryResult(this, queryOptions);

        public override StorageFileQueryResult CreateFileQuery() => throw new NotSupportedException();
        public override StorageFileQueryResult CreateFileQuery(CommonFileQuery query) => throw new NotSupportedException();
        public override IBaseStorageFileQueryResult CreateFileQueryWithOptions(QueryOptions queryOptions) => new BaseStorageFileQueryResult(this, queryOptions);

        public override StorageFolderQueryResult CreateFolderQuery() => throw new NotSupportedException();
        public override StorageFolderQueryResult CreateFolderQuery(CommonFolderQuery query) => throw new NotSupportedException();
        public override IBaseStorageFolderQueryResult CreateFolderQueryWithOptions(QueryOptions queryOptions) => new BaseStorageFolderQueryResult(this, queryOptions);

        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (IsShellPath(Path))
                {
                    return null;
                }
                var zipFolder = await StorageFolder.GetFolderFromPathAsync(Path);
                return await zipFolder.GetThumbnailAsync(mode);
            });
        }
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (IsShellPath(Path))
                {
                    return null;
                }
                var zipFolder = await StorageFolder.GetFolderFromPathAsync(Path);
                return await zipFolder.GetThumbnailAsync(mode, requestedSize);
            });
        }
        public override IAsyncOperation<StorageItemThumbnail> GetThumbnailAsync(ThumbnailMode mode, uint requestedSize, ThumbnailOptions options)
        {
            return AsyncInfo.Run(async (cancellationToken) =>
            {
                if (IsShellPath(Path))
                {
                    return null;
                }
                var zipFolder = await StorageFolder.GetFolderFromPathAsync(Path);
                return await zipFolder.GetThumbnailAsync(mode, requestedSize, options);
            });
        }
    }
}
