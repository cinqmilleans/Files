﻿using Files.Extensions;
using Files.Filesystem.StorageItems;
using System;
using System.Collections.Concurrent;
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
    public static class FolderHelpers
    {
        private static readonly ConcurrentDictionary<string, long?> cacheSizes = new();

        public static bool CheckFolderAccessWithWin32(string path)
        {
            FINDEX_INFO_LEVELS findInfoLevel = FINDEX_INFO_LEVELS.FindExInfoBasic;
            int additionalFlags = FIND_FIRST_EX_LARGE_FETCH;
            IntPtr hFileTsk = FindFirstFileExFromApp(path + "\\*.*", findInfoLevel, out WIN32_FIND_DATA findDataTsk, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero,
                additionalFlags);
            if (hFileTsk.ToInt64() != -1)
            {
                FindClose(hFileTsk);
                return true;
            }
            return false;
        }

        public static async Task<bool> CheckBitlockerStatusAsync(BaseStorageFolder rootFolder, string path)
        {
            if (rootFolder == null || rootFolder.Properties == null)
            {
                return false;
            }
            if (Path.IsPathRooted(path) && Path.GetPathRoot(path) == path)
            {
                IDictionary<string, object> extraProperties = await rootFolder.Properties.RetrievePropertiesAsync(new string[] { "System.Volume.BitLockerProtection" });
                return (int?)extraProperties["System.Volume.BitLockerProtection"] == 6; // Drive is bitlocker protected and locked
            }
            return false;
        }

        /// <summary>
        /// This function is used to determine whether or not a folder has any contents.
        /// </summary>
        /// <param name="targetPath">The path to the target folder</param>
        ///
        public static bool CheckForFilesFolders(string targetPath)
        {
            FINDEX_INFO_LEVELS findInfoLevel = FINDEX_INFO_LEVELS.FindExInfoBasic;
            int additionalFlags = FIND_FIRST_EX_LARGE_FETCH;

            IntPtr hFile = FindFirstFileExFromApp(targetPath + "\\*.*", findInfoLevel, out WIN32_FIND_DATA _, FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, additionalFlags);
            FindNextFile(hFile, out _);
            var result = FindNextFile(hFile, out _);
            FindClose(hFile);
            return result;
        }

        public static async void UpdateFolder(ListedItem folder, CancellationToken cancellationToken)
        {
            CoreDispatcher dispatcher;

            if (folder.PrimaryItemAttribute == Windows.Storage.StorageItemTypes.Folder && folder.ContainsFilesOrFolders)
            {
                dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                long size = await Calculate(folder.ItemPath);
                await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    if (size > folder.FileSizeBytes)
                    {
                        folder.FileSizeBytes = size;
                        folder.FileSize = size.ToSizeString();
                    };
                });
            }

            async Task<long> Calculate(string folderPath)
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return 0;
                }

                cacheSizes.TryGetValue(folderPath, out long? cacheSize);
                if (cacheSize.HasValue)
                {
                    return cacheSize.Value;
                }

                FINDEX_INFO_LEVELS findInfoLevel = FINDEX_INFO_LEVELS.FindExInfoBasic;
                int additionalFlags = FIND_FIRST_EX_LARGE_FETCH;

                IntPtr hFile = FindFirstFileExFromApp(folderPath + "\\*.*", findInfoLevel, out WIN32_FIND_DATA findData,
                                    FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, additionalFlags);

                long size = 0;
                string localPath = string.Empty;
                long localSize = 0;
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
                                localPath = Path.Combine(folderPath, findData.cFileName);
                                localSize = await Calculate(localPath);
                                size += localSize;
                            }
                        }

                        await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            cacheSizes.TryAdd(localPath, localSize);
                            if (size > folder.FileSizeBytes)
                            {
                                folder.FileSizeBytes = size;
                                folder.FileSize = size.ToSizeString();
                            };
                        });

                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    } while (FindNextFile(hFile, out findData));
                    FindClose(hFile);
                    return size;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
