using ICSharpCode.SharpZipLib.Zip;
using System;

namespace Files.Backend.Filesystem.Storage
{
    internal class ZipBasicProperties : BaseStorageItemExtraProperties, IBaseBasicProperties
    {
        private readonly ZipEntry entry;

        public ulong Size => (ulong)entry.Size;

        public DateTimeOffset ItemDate => entry.DateTime;
        public DateTimeOffset DateModified => entry.DateTime;

        public ZipBasicProperties(ZipEntry entry) => this.entry = entry;
    }
}
