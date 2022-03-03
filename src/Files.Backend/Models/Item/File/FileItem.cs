using CommunityToolkit.Mvvm.ComponentModel;
using Files.BackEnd;
using System;
using System.ComponentModel;

namespace Files.Backend.Models.Item
{
    public interface IFileItem : IItem
    {
        ByteSize Size { get; }

        DateTime DateCreated { get; }
        DateTime DateModified { get; }
        DateTime DateAccessed { get; }
    }

    public class FileItem : ObservableObject, IFileItem
    {
        public string Path { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;

        public ByteSize Size { get; init; } = ByteSize.Zero;

        public DateTime DateCreated { get; init; }
        public DateTime DateModified { get; init; }
        public DateTime DateAccessed { get; init; }
    }
}
