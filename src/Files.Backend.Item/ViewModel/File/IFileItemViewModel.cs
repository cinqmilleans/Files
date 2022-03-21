using System;

namespace Files.Backend.Item
{
    public interface IFileItemViewModel : IItemViewModel
    {
        string Extension { get; }

        bool IsArchive { get; }
        bool IsCompressed { get; }
        bool IsDevice { get; }
        bool IsDirectory { get; }
        bool IsEncrypted { get; }
        bool IsHidden { get; }
        bool IsOffline { get; }
        bool IsReadOnly { get; }
        bool IsSystem { get; }
        bool IsTemporary { get; }
        bool IsExecutable { get; }

        bool IsShortcutItem { get; }
        bool IsSymbolicLinkShortcutItem { get; }
        bool IsUrlShortcutItem { get; }
        bool IsLibraryItem { get; }
        bool IsRecycleBinItem { get; }
        bool IsFtpItem { get; }
        bool IsZipItem { get; }

        ByteSize Size { get; }

        DateTime DateCreated { get; }
        DateTime DateModified { get; }
        DateTime DateAccessed { get; }

        IShortcutViewModel? Shortcut { get; }
        ILibraryViewModel? Library { get; }
    }
}
