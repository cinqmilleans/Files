using Files.Backend.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.FileProperties;

namespace Files.Backend.Models.Storage
{
    internal class BaseStorageItemExtraProperties : IStorageItemExtraProperties
    {
        public virtual IAsyncAction SavePropertiesAsync()
            => Task.CompletedTask.AsAsyncAction();
        public virtual IAsyncAction SavePropertiesAsync([HasVariant] IEnumerable<KeyValuePair<string, object>> propertiesToSave)
            => Task.CompletedTask.AsAsyncAction();

        public virtual IAsyncOperation<IDictionary<string, object?>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
        {
            return AsyncInfo.Run((cancellationToken) =>
            {
                var props = new Dictionary<string, object?>();
                propertiesToRetrieve.ForEach(x => props[x] = null);
                return Task.FromResult<IDictionary<string, object?>>(props);
            });
        }
    }
}
