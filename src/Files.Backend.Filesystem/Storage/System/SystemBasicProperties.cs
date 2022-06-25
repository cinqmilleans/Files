using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.FileProperties;

namespace Files.Backend.Filesystem.Storage
{
    internal class SystemBasicProperties : IBaseBasicProperties
    {
        private readonly BasicProperties? properties;

        public ulong Size => properties?.Size ?? 0;

        public DateTimeOffset ItemDate => properties?.ItemDate ?? DateTimeOffset.Now;
        public DateTimeOffset DateModified => properties?.DateModified ?? DateTimeOffset.Now;

        public SystemBasicProperties(BasicProperties? properties) => this.properties = properties;

        public IAsyncOperation<IDictionary<string, object>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
            => properties.RetrievePropertiesAsync(propertiesToRetrieve);

        public IAsyncAction SavePropertiesAsync()
            => properties.SavePropertiesAsync();
        public IAsyncAction SavePropertiesAsync([HasVariant] IEnumerable<KeyValuePair<string, object>> propertiesToSave)
            => properties.SavePropertiesAsync(propertiesToSave);
    }
}
