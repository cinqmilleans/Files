﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Files.Backend.Models.Storage
{
    internal class SystemStorageItemQueryResult : BaseStorageItemQueryResult
    {
        private StorageItemQueryResult StorageItemQueryResult { get; }

        public SystemStorageItemQueryResult(StorageItemQueryResult sfqr) : base(sfqr.Folder, sfqr.GetCurrentQueryOptions())
        {
            StorageItemQueryResult = sfqr;
        }

        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync(uint startIndex, uint maxNumberOfItems)
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync(startIndex, maxNumberOfItems);
                return items.Select(x => x is StorageFolder ? (IStorageItem)new SystemStorageFolder(x as StorageFolder) : new SystemStorageFile(x as StorageFile)).ToList();
            });
        }

        public override IAsyncOperation<IReadOnlyList<IStorageItem>> GetItemsAsync()
        {
            return AsyncInfo.Run<IReadOnlyList<IStorageItem>>(async (cancellationToken) =>
            {
                var items = await StorageItemQueryResult.GetItemsAsync();
                return items.Select(x => x is StorageFolder ? (IStorageItem)new SystemStorageFolder(x as StorageFolder) : new SystemStorageFile(x as StorageFile)).ToList();
            });
        }

        public override StorageItemQueryResult ToStorageItemQueryResult() => StorageItemQueryResult;
    }
}
