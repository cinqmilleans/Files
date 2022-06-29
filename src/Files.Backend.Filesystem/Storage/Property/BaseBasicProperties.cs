using System;

namespace Files.Backend.Filesystem.Storage
{
    public class BaseBasicProperties : BaseStorageItemExtraProperties, IBaseBasicProperties
    {
        public ulong Size => 0;

        public DateTimeOffset ItemDate => DateTimeOffset.Now;
        public DateTimeOffset DateModified => DateTimeOffset.Now;
    }
}
