using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Attribute = System.IO.FileAttributes;
using static Files.Backend.Item.Tools.NativeFindStorageItemHelper;
using Files.Backend.Item.Tools;

namespace Files.Backend.Item
{
    /*internal class ShortcutFactory
    {
        public IShortcut? Build (string path, WIN32_FIND_DATA data)
        {
            var type = GetShortcutType(data);
            if (type is ShortcutTypes.Unknown)
            {
                return null;
            }

            string targetPath = GetTargetPath(type, path);

            return new Shortcut
            {
                ShortcutType = type,
                TargetPath = targetPath,
                Arguments = string.Empty,
                WorkingDirectory = string.Empty,
            };
        }


        private ShortcutTypes GetShortcutType(WIN32_FIND_DATA data)
        {
            bool isReparsePoint = ((Attribute)data.dwFileAttributes & Attribute.ReparsePoint) is Attribute.ReparsePoint;
            if (isReparsePoint && data.dwReserved0 is NativeFileOperationsHelper.IO_REPARSE_TAG_SYMLINK)
            {
                return ShortcutTypes.SymbolicLink;
            }
            return ShortcutTypes.Unknown;
        }

        private string GetTargetPath(ShortcutTypes shortcutType, string path) => shortcutType switch
        {
            ShortcutTypes.SymbolicLink => NativeFileOperationsHelper.ParseSymLink(path),
            _ => string.Empty,
        };
    }*/
}
