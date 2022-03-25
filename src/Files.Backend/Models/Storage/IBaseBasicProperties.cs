using System;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseBasicProperties : IStorageItemExtraProperties
    {
        ulong Size { get; }

        DateTimeOffset DateCreated { get; }
        DateTimeOffset DateModified { get; }
    }
}
