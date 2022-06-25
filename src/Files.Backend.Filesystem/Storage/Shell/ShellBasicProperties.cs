using Files.Shared;
using System;

namespace Files.Backend.Filesystem.Storage
{
    internal class ShellBasicProperties : BaseStorageItemExtraProperties, IBaseBasicProperties
    {
        private readonly ShellFileItem file;

        public ShellBasicProperties(ShellFileItem folder) => this.file = folder;

        public ulong Size => file.FileSizeBytes;

        public DateTimeOffset ItemDate => file.ModifiedDate;
        public DateTimeOffset DateModified => file.ModifiedDate;
    }
}
