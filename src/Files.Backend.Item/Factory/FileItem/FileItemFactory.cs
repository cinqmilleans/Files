using Files.Backend.Item.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Windows.UI.Xaml.Media.Imaging;
using static Files.Backend.Item.Tools.NativeFindStorageItemHelper;

namespace Files.Backend.Item
{
    /*internal class FileItemFactory : IFileItemFactory
    {
        private readonly string folderPath;
        private WIN32_FIND_DATA data;

        public FileItemFactory(string folderPath, WIN32_FIND_DATA data)
            => (this.folderPath, this.data) = (folderPath, data);

        public IFileItem Build()
        {
            try
            {
                return BuildItem();
            }
            catch (Exception ex)
            {
                var error = GetError(ex);
                throw new ItemException(error, ex);
            }
        }

        private IFileItem BuildItem()
        {
            string name = data.cFileName;
            string path = folderPath.CombineNameToPath(name);

            DateTime dateCreated, dateModified, dateAccessed;
            try
            {
                dateCreated = Win32FindDataExtension.ToDateTime(ref data.ftCreationTime);
                dateModified = Win32FindDataExtension.ToDateTime(ref data.ftLastWriteTime);
                dateAccessed = Win32FindDataExtension.ToDateTime(ref data.ftLastAccessTime);
            }
            catch (InvalidCastException ex)
            {
                throw new ItemException();
            }

            long itemSizeBytes = data.GetSize();
            var itemSize = itemSizeBytes.ToSizeString();
            string itemType = "ItemTypeFile".GetLocalized();
            string itemFileExtension = null;

            if (findData.cFileName.Contains('.'))
            {
                itemFileExtension = Path.GetExtension(itemPath);
                itemType = itemFileExtension.Trim('.') + " " + itemType;
            }

            bool itemThumbnailImgVis = false;
            bool itemEmptyImgVis = true;

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            bool isHidden = ((FileAttributes)findData.dwFileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden;
            double opacity = isHidden ? Constants.UI.DimItemOpacity : 1;

            // https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-fscc/c8e77b37-3909-4fe6-a4ea-2b9d423b1ee4
            bool isReparsePoint = ((FileAttributes)findData.dwFileAttributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
            bool isSymlink = isReparsePoint && findData.dwReserved0 == NativeFileOperationsHelper.IO_REPARSE_TAG_SYMLINK;

            if (isSymlink)
            {
                var targetPath = NativeFileOperationsHelper.ParseSymLink(itemPath);

            }
            else if (findData.cFileName.EndsWith(".lnk", StringComparison.Ordinal) || findData.cFileName.EndsWith(".url", StringComparison.Ordinal))
            {
                if (connection != null)
                {
                    var (status, response) = await connection.SendMessageForResponseAsync(new ValueSet()
                    {
                        { "Arguments", "FileOperation" },
                        { "fileop", "ParseLink" },
                        { "filepath", itemPath }
                    });
                    // If the request was canceled return now
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    if (status == AppServiceResponseStatus.Success && response.ContainsKey("ShortcutInfo"))
                    {
                        var isUrl = findData.cFileName.EndsWith(".url", StringComparison.OrdinalIgnoreCase);
                        var shInfo = JsonConvert.DeserializeObject<ShellLinkItem>((string)response["ShortcutInfo"]);
                        if (shInfo == null)
                        {
                            return null;
                        }
                        return new ShortcutItem(null, dateReturnFormat)
                        {
                            PrimaryItemAttribute = shInfo.IsFolder ? StorageItemTypes.Folder : StorageItemTypes.File,
                            FileExtension = itemFileExtension,
                            IsHiddenItem = isHidden,
                            Opacity = opacity,
                            FileImage = null,
                            LoadFileIcon = !shInfo.IsFolder && itemThumbnailImgVis,
                            LoadWebShortcutGlyph = !shInfo.IsFolder && isUrl && itemEmptyImgVis,
                            ItemNameRaw = itemName,
                            ItemDateModifiedReal = itemModifiedDate,
                            ItemDateAccessedReal = itemLastAccessDate,
                            ItemDateCreatedReal = itemCreatedDate,
                            ItemType = isUrl ? "ShortcutWebLinkFileType".GetLocalized() : "ShortcutFileType".GetLocalized(),
                            ItemPath = itemPath,
                            FileSize = itemSize,
                            FileSizeBytes = itemSizeBytes,
                            TargetPath = shInfo.TargetPath,
                            Arguments = shInfo.Arguments,
                            WorkingDirectory = shInfo.WorkingDirectory,
                            RunAsAdmin = shInfo.RunAsAdmin,
                            IsUrl = isUrl,
                        };
                    }
                }
            }
            else if (App.LibraryManager.TryGetLibrary(itemPath, out LibraryLocationItem library))
            {
                return new LibraryItem(library)
                {
                    ItemDateModifiedReal = itemModifiedDate,
                    ItemDateCreatedReal = itemCreatedDate,
                };
            }
            else
            {
                if (".zip".Equals(itemFileExtension, StringComparison.OrdinalIgnoreCase) && await ZipStorageFolder.CheckDefaultZipApp(itemPath))
                {
                    return new ZipItem(null, dateReturnFormat)
                    {
                        PrimaryItemAttribute = StorageItemTypes.Folder, // Treat zip files as folders
                        FileExtension = itemFileExtension,
                        FileImage = null,
                        LoadFileIcon = itemThumbnailImgVis,
                        ItemNameRaw = itemName,
                        IsHiddenItem = isHidden,
                        Opacity = opacity,
                        ItemDateModifiedReal = itemModifiedDate,
                        ItemDateAccessedReal = itemLastAccessDate,
                        ItemDateCreatedReal = itemCreatedDate,
                        ItemType = itemType,
                        ItemPath = itemPath,
                        FileSize = itemSize,
                        FileSizeBytes = itemSizeBytes
                    };
                }
                else
                {
                    return new ListedItem(null, dateReturnFormat)
                    {
                        PrimaryItemAttribute = StorageItemTypes.File,
                        FileExtension = itemFileExtension,
                        FileImage = null,
                        LoadFileIcon = itemThumbnailImgVis,
                        ItemNameRaw = itemName,
                        IsHiddenItem = isHidden,
                        Opacity = opacity,
                        ItemDateModifiedReal = itemModifiedDate,
                        ItemDateAccessedReal = itemLastAccessDate,
                        ItemDateCreatedReal = itemCreatedDate,
                        ItemType = itemType,
                        ItemPath = itemPath,
                        FileSize = itemSize,
                        FileSizeBytes = itemSizeBytes
                    };
                }
            }
            return null;

        }

        private IShortcut? GetShortcut()
        {

        }

        private static ItemErrors GetError (Exception ex) => ex switch
        {
            UnauthorizedAccessException => ItemErrors.Unauthorized,
            FileNotFoundException => ItemErrors.NotFound,
            PathTooLongException => ItemErrors.NameTooLong,
            _ => ItemErrors.Unknown,
        };

        private class Item
        {
            public string Path = string.Empty;
            public string Name = string.Empty;
            public string Extension = string.Empty;

            public FileAttributes FileAttribute = FileAttributes.None;

            public ByteSize Size = ByteSize.Zero;

            public DateTime DateCreated = DateTime.MinValue;
            public DateTime DateModified = DateTime.MinValue;
            public DateTime DateAccessed = DateTime.MinValue;

            public BitmapImage? MainImage;
            public BitmapImage? OverlayImage;
        }
    }*/
}
