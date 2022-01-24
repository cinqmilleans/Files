using Files.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using static Files.Helpers.NativeFindStorageItemHelper;

namespace Files.Filesystem
{
    public interface ISizeProvider
    {
        Task<long> GetSize(string path, CancellationToken cancellationToken);
    }

    public class DeviceSizeProvider : FolderSizeProvider
    {
    }

    public class FolderSizeProvider : ISizeProvider
    {
        public IDictionary<string, FolderSizeProvider> children;

        private long? size;

        public async Task<long> GetSize(string path, CancellationToken cancellationToken)
        {
            var provider = GetProvider(path);
            return await Update(path, cancellationToken);
        }

        private FolderSizeProvider GetProvider(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                return this;
            }
            var root = Path.GetPathRoot(path);
            path = Path.GetRelativePath(root, path);
            if (children is not null && children.ContainsKey(root))
            {
                return children[root].GetProvider(path);
            }
            var child = new FolderSizeProvider();
            if (children is null)
            {
                children = new Dictionary<string, FolderSizeProvider>();
            }
            children.Add(root, child);
            return child.GetProvider(path);
        }

        private async Task<long> Update(string path, CancellationToken cancellationToken)
        {
            long size = 0;
            HashSet<string> childNames = children is not null ? children.Keys.ToHashSet() : null;

            bool isNew = false;
            string childName = null;
            FolderSizeProvider childProvider = null;

            IntPtr hFile = FindFirstFileExFromApp(path + "\\*.*", FINDEX_INFO_LEVELS.FindExInfoBasic,
                out WIN32_FIND_DATA findData, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, FIND_FIRST_EX_LARGE_FETCH);
            if (hFile.ToInt64() != -1)
            {
                do
                {
                    if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        size += findData.GetSize();
                    }
                    else if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        if (findData.cFileName is not "." and not "..")
                        {
                            childName = findData.cFileName;
                            string childPath = Path.Combine(path, childName);
                            if (childNames.Contains(childName))
                            {
                                childProvider = children[childName];
                                childNames.Remove(childName);
                            }
                            else
                            {
                                childProvider = new FolderSizeProvider();
                                if (children is null)
                                children.Add(childName, childProvider);
                            }
                            long? childSize = await childProvider.Update(path, cancellationToken);
                            if (childSize.HasValue)
                            {
                                size += childSize.Value;
                            }
                        }
                    }

                    var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        if (isNew)
                        {
                            if (children is null)
                            {
                                children = new Dictionary<string, FolderSizeProvider>();
                            }
                            children.Add(childName, childProvider);
                        }
                        if (size > this.size)
                        {
                            this.size = size;
                        };
                    });

                    isNew = false;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                } while (FindNextFile(hFile, out findData));
                FindClose(hFile);
            }
            return size;
        }
    }
}
