using System;
using Windows.Storage.FileProperties;

namespace Files.Backend.Storage
{
    public interface IBaseBasicProperties : IStorageItemExtraProperties
    {
        ulong Size { get; }

        DateTimeOffset ItemDate { get; }
        DateTimeOffset DateModified { get; }
    }
}
