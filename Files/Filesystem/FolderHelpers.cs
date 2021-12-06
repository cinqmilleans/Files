using ByteSizeLib;
using Files.Extensions;
using Files.Filesystem.StorageItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using static Files.Helpers.NativeFindStorageItemHelper;

namespace Files.Filesystem
{
    public static class FolderHelpers
    {
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

        public static async void UpdateFolder(ListedItem item, CancellationToken cancellationToken)
        {
            CoreDispatcher dispatcher;

            //long size = 0;

            if (item.ContainsFilesOrFolders)
            {
                dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                //item.FileSizeBytes = 0;
                //item.FileSize = string.Empty;

                _ = await Calculate(item.ItemPath, false).ConfigureAwait(false);
                //item.FileSize = GetSizeString(size);
            }

            static string GetSizeString(long size) => ByteSize.FromBytes(size).ToBinaryString().ConvertSizeAbbreviation();

            async Task<long> Calculate(string path, bool b)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return 0;
                }

                FINDEX_INFO_LEVELS findInfoLevel = FINDEX_INFO_LEVELS.FindExInfoBasic;
                int additionalFlags = FIND_FIRST_EX_LARGE_FETCH;

                IntPtr hFile = FindFirstFileExFromApp(path + "\\*.*", findInfoLevel, out WIN32_FIND_DATA findData,
                                    FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, additionalFlags);

                long size = 0;
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
                                string itemPath = Path.Combine(path, findData.cFileName);
                                size += await Calculate(itemPath, true).ConfigureAwait(false);
                            }
                        }

                        if (size > item.FileSizeBytes)
                        {
                            await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                            {
                                item.FileSizeBytes = size;
                                //if (b) item.FileSize = GetSizeString(size);
                            });
                        }

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