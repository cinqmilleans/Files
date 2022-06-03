using Files.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using static Files.Backend.Helpers.NativeFindStorageItemHelper;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizedPathEnumerator : ISizedPathEnumerator
    {
        public async IAsyncEnumerable<Folder> EnumerateSizedFolders(string path, uint level = 0, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IntPtr hFile = FindFirstFileExFromApp($"{path}{Path.DirectorySeparatorChar}*.*", FINDEX_INFO_LEVELS.FindExInfoBasic,
                out WIN32_FIND_DATA findData, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, FIND_FIRST_EX_LARGE_FETCH);

            ulong localSize = 0;
            ulong globalSize = 0;

            if (hFile.ToInt64() != -1)
            {
                do
                {
                    bool isDirectory = ((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) is FileAttributes.Directory;
                    if (!isDirectory)
                    {

                        localSize += (ulong)findData.GetSize();
                    }
                    else if (findData.cFileName is not "." and not "..")
                    {
                        var subFolders = EnumerateSizedFolders(Path.Combine(path, findData.cFileName), level + 1).WithCancellation(cancellationToken);
                        await foreach (var subFolder in subFolders)
                        {
                            globalSize += subFolder.GlobalSize;
                            yield return subFolder;
                        }
                    }
                    globalSize += localSize;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                } while (FindNextFile(hFile, out findData));
                FindClose(hFile);
            }
            yield return new(path, level, localSize, globalSize);
        }
    }
}
