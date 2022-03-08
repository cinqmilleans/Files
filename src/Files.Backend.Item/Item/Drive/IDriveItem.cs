using Files.Shared;
using System;

namespace Files.Backend.Item
{
    public interface IDriveItem : IItem
    {
        DriveTypes DriveType { get; }

        ByteSize UsedSpace { get; }
        ByteSize FreeSpace { get; }
        ByteSize TotalSpace { get; }

        Uri? IconSource { get; }
        byte[]? IconImage { get; }
    }
}
