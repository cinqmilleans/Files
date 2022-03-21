using System;
using System.Collections.Generic;
using System.Text;
using Attributes = System.IO.FileAttributes;
using static Files.Backend.Item.Tools.NativeFindStorageItemHelper;

namespace Files.Backend.Item
{
    /*private class ShortcutItemBuilder
    {
        private const uint IO_REPARSE_TAG_SYMLINK = 0xA000000C;

        public bool CanBuild(WIN32_FIND_DATA data)
        {
            bool isReparsePoint = ((Attributes)data.dwFileAttributes & Attributes.ReparsePoint) == Attributes.ReparsePoint;
            return isReparsePoint && data.dwReserved0 == IO_REPARSE_TAG_SYMLINK;

        }

        public IShortcutItem Build(string path, WIN32_FIND_DATA data)
        {
            var targetPath = NativeFileOperationsHelper.ParseSymLink(itemPath);
            return new ShortcutItem(null, dateReturnFormat)
            {
                PrimaryItemAttribute = StorageItemTypes.File,
                FileExtension = itemFileExtension,
                IsHiddenItem = isHidden,
                Opacity = opacity,
                FileImage = null,
                LoadFileIcon = itemThumbnailImgVis,
                LoadWebShortcutGlyph = false,
                ItemNameRaw = itemName,
                ItemDateModifiedReal = itemModifiedDate,
                ItemDateAccessedReal = itemLastAccessDate,
                ItemDateCreatedReal = itemCreatedDate,
                ItemType = "ShortcutFileType".GetLocalized(),
                ItemPath = itemPath,
                FileSize = itemSize,
                FileSizeBytes = itemSizeBytes,
                TargetPath = targetPath,
                IsSymLink = true
            };
        }
    }*/
}
