using System;

namespace Files.Backend.Models.Storage
{
    internal class BaseBasicProperties : BaseStorageItemExtraProperties, IBaseBasicProperties
    {
        public virtual ulong Size => 0L;

        public virtual DateTimeOffset DateCreated => DateTimeOffset.Now;
        public virtual DateTimeOffset DateModified => DateTimeOffset.Now;
    }
}
