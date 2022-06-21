using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.FileProperties;

namespace Files.Backend.Storage
{
    internal class SystemFolderBasicProperties : IBaseBasicProperties
    {
        private readonly IStorageItemExtraProperties basicProps;

        public ulong Size => (basicProps as BasicProperties)?.Size ?? 0;

        public DateTimeOffset ItemDate => (basicProps as BasicProperties)?.ItemDate ?? DateTimeOffset.Now;
        public DateTimeOffset DateModified => (basicProps as BasicProperties)?.DateModified ?? DateTimeOffset.Now;

        public SystemFolderBasicProperties(IStorageItemExtraProperties basicProps) => this.basicProps = basicProps;

        public IAsyncOperation<IDictionary<string, object>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
            => basicProps.RetrievePropertiesAsync(propertiesToRetrieve);

        public IAsyncAction SavePropertiesAsync()
            => basicProps.SavePropertiesAsync();
        public IAsyncAction SavePropertiesAsync([HasVariant] IEnumerable<KeyValuePair<string, object>> propertiesToSave)
            => basicProps.SavePropertiesAsync(propertiesToSave);
    }
}
