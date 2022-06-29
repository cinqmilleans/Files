using Files.Shared;
using System;

namespace Files.Backend.Filesystem.Storage
{
    public class BinStorageFile : ShellStorageFile, IBinStorageItem
    {
        public string OriginalPath { get; }
        public DateTimeOffset DateDeleted { get; }

        public BinStorageFile(ShellFileItem item) : base(item)
        {
            OriginalPath = item.FilePath;
            DateDeleted = item.RecycleDate;
        }
    }
}
