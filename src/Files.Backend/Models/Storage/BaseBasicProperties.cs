using System;

namespace Files.Backend.Models.Storage
{
    internal class BaseBasicProperties : BaseStorageItemExtraProperties, IBasicProperties
    {
        public virtual ulong Size => 0;

        public virtual DateTimeOffset DateCreated => DateTimeOffset.Now;
        public virtual DateTimeOffset DateModified => DateTimeOffset.Now;
        public virtual DateTimeOffset DateAccessed => DateTimeOffset.Now;
    }
}
