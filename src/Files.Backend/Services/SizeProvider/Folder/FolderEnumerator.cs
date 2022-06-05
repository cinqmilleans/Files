using Files.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Files.Backend.Helpers.NativeFindStorageItemHelper;

namespace Files.Backend.Services.SizeProvider
{
    internal class FolderEnumerator : IFolderEnumerator
    {
        public async IAsyncEnumerable<IFolder> EnumerateFolders(string path, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var folders = EnumerateFolders(path, 0, cancellationToken).WithCancellation(cancellationToken);
            await foreach (var folder in folders)
            {
                yield return folder;
            }
        }
        private async IAsyncEnumerable<IFolder> EnumerateFolders
            (string path, uint level, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IntPtr hFile = FindFirstFileExFromApp($"{path}{Path.DirectorySeparatorChar}*.*", FINDEX_INFO_LEVELS.FindExInfoBasic,
                out WIN32_FIND_DATA findData, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, FIND_FIRST_EX_LARGE_FETCH);

            var folder = new Folder(path, level);

            if (hFile.ToInt64() is not -1)
            {
                do
                {
                    bool isDirectory = ((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) is FileAttributes.Directory;
                    if (!isDirectory)
                    {
                        folder.LocalSize += (ulong)findData.GetSize();
                    }
                    else if (findData.cFileName is not "." and not "..")
                    {
                        var subFolders = EnumerateFolders(Path.Combine(path, findData.cFileName), level + 1).WithCancellation(cancellationToken);
                        await foreach (var subFolder in subFolders)
                        {
                            folder.GlobalSize += subFolder.LocalSize;
                            yield return subFolder;
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                } while (FindNextFile(hFile, out findData));
                FindClose(hFile);
            }

            folder.GlobalSize += folder.LocalSize;
            yield return folder;
        }

        [DebuggerDisplay("{Path},{LocalSize},{GlobalSize}")]
        private class Folder : IFolder
        {
            public string Path { get; }
            public uint Level { get; }

            public ulong LocalSize { get; set; }
            public ulong GlobalSize { get; set; }

            public Folder(string path, uint level) => (Path, Level) = (path, level);
        }
    }
}
