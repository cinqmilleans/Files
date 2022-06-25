using Files.Shared;
using System;

namespace Files.Backend.Filesystem.Storage
{
    public class BinStorageFolder : ShellStorageFolder, IBinStorageItem
    {
        public string OriginalPath { get; }
        public DateTimeOffset DateDeleted { get; }

        public BinStorageFolder(ShellFileItem item) : base(item)
        {
            OriginalPath = item.FilePath;
            DateDeleted = item.RecycleDate;
        }
    }
}
