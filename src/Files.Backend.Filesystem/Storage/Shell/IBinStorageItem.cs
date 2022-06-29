using System;
using Windows.Storage;

namespace Files.Backend.Filesystem.Storage
{
    public interface IBinStorageItem : IStorageItem
    {
        string OriginalPath { get; }
        DateTimeOffset DateDeleted { get; }
    }
}
