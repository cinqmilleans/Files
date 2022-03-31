using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal class BaseStorageItemExtraProperties : IStorageItemExtraProperties
    {
        public virtual IAsyncOperation<IDictionary<string, object>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
        {
            return AsyncInfo.Run<IDictionary<string, object>>((cancellationToken) =>
            {
                var props = new Dictionary<string, object>();
                propertiesToRetrieve.ForEach(x => props[x] = null);
                return Task.FromResult<IDictionary<string, object>>(props);
            });
        }

        public virtual IAsyncAction SavePropertiesAsync([HasVariant] IEnumerable<KeyValuePair<string, object>> propertiesToSave)
        {
            return Task.CompletedTask.AsAsyncAction();
        }

        public virtual IAsyncAction SavePropertiesAsync()
        {
            return Task.CompletedTask.AsAsyncAction();
        }
    }
}
