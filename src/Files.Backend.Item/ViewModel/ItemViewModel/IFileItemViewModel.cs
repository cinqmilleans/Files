using Files.Shared;
using System;

namespace Files.Backend.Item
{
    public interface IFileItemViewModel : IItemViewModel
    {
        bool IsArchive => false;
        bool IsCompressed => false;
        bool IsDevice => false;
        bool IsDirectory => false;
        bool IsEncrypted => false;
        bool IsHidden => false;
        bool IsOffline => false;
        bool IsReadOnly => false;
        bool IsSystem => false;
        bool IsTemporary => false;

        bool IsShortcutItem => false;
        bool IsExecutableShortcutItem => false;
        bool IsSymbolicLinkShortcutItem => false;
        bool IsUrlShortcutItem => false;
        bool IsLibraryItem => false;

        ByteSize Size => ByteSize.Zero;

        DateTime DateCreated => DateTime.MinValue;
        DateTime DateModified => DateTime.MinValue;
        DateTime DateAccessed => DateTime.MinValue;

        IShortcutViewModel? Shortcut => null;
        ILibraryViewModel? Library => null;
    }
}
