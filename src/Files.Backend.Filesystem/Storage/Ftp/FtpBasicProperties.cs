using FluentFTP;
using System;

namespace Files.Backend.Filesystem.Storage
{
    internal class FtpBasicProperties : BaseStorageItemExtraProperties, IBaseBasicProperties
    {
        public ulong Size { get; }

        public DateTimeOffset ItemDate { get; }
        public DateTimeOffset DateModified { get; }

        public FtpBasicProperties(FtpListItem item)
        {
            Size = (ulong)item.Size;

            ItemDate = item.RawCreated < DateTime.FromFileTimeUtc(0) ? DateTimeOffset.MinValue : item.RawCreated;
            DateModified = item.RawModified < DateTime.FromFileTimeUtc(0) ? DateTimeOffset.MinValue : item.RawModified;
        }
    }
}
