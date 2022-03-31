using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;
using Windows.Storage.Search;

namespace Files.Backend.Models.Storage
{
    internal interface IBaseStorageFileQueryResult : IStorageQueryResultBase
    {
        public extern IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync([In] uint startIndex, [In] uint maxNumberOfItems);

        IAsyncOperation<IReadOnlyList<StorageFile>> IStorageFileQueryResult.GetFilesAsync([In] uint startIndex, [In] uint maxNumberOfItems)
        public extern IAsyncOperation<IReadOnlyList<StorageFile>> GetFilesAsync();

        IAsyncOperation<IReadOnlyList<StorageFile>> IStorageFileQueryResult.GetFilesAsync()
        public extern IAsyncOperation<uint> GetItemCountAsync();

        IAsyncOperation<uint> IStorageQueryResultBase.GetItemCountAsync()

        public extern IAsyncOperation<uint> FindStartIndexAsync([In][Variant] object value);

        IAsyncOperation<uint> IStorageQueryResultBase.FindStartIndexAsync([In][Variant] object value)
        {
            //ILSpy generated this explicit interface implementation from .override directive in FindStartIndexAsync
            return this.FindStartIndexAsync(value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern QueryOptions GetCurrentQueryOptions();

        QueryOptions IStorageQueryResultBase.GetCurrentQueryOptions()
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetCurrentQueryOptions
            return this.GetCurrentQueryOptions();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void ApplyNewQueryOptions([In] QueryOptions newQueryOptions);

        void IStorageQueryResultBase.ApplyNewQueryOptions([In] QueryOptions newQueryOptions)
        {
            //ILSpy generated this explicit interface implementation from .override directive in ApplyNewQueryOptions
            this.ApplyNewQueryOptions(newQueryOptions);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern IDictionary<string, IReadOnlyList<TextSegment>> GetMatchingPropertiesWithRanges([In] StorageFile file);

        IDictionary<string, IReadOnlyList<TextSegment>> IStorageFileQueryResult2.GetMatchingPropertiesWithRanges([In] StorageFile file)
        {
            //ILSpy generated this explicit interface implementation from .override directive in GetMatchingPropertiesWithRanges
            return this.GetMatchingPropertiesWithRanges(file);
        }
    }
}
