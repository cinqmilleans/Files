using System;

namespace Files.Backend.Filesystem.Storage
{
    public class BaseBasicProperties : BaseStorageItemExtraProperties
    {
        public virtual ulong Size => 0;

        public virtual DateTimeOffset ItemDate => DateTimeOffset.Now;
        public virtual DateTimeOffset DateModified => DateTimeOffset.Now;
    }
}
