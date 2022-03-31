using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.Storage;

namespace Files.Backend.Models.Storage
{
    internal class BasicStorageItemExtraProperties : BaseStorageItemExtraProperties
    {
        private IStorageItem item;

        public BasicStorageItemExtraProperties(IStorageItem item)
        {
            this.item = item;
        }

        public override IAsyncOperation<IDictionary<string, object>> RetrievePropertiesAsync(IEnumerable<string> propertiesToRetrieve)
        {
            return AsyncInfo.Run<IDictionary<string, object>>(async (cancellationToken) =>
            {
                var props = new Dictionary<string, object>();
                propertiesToRetrieve.ForEach(x => props[x] = null);
                // Fill common poperties
                var ret = item.AsBaseStorageFile()?.GetBasicPropertiesAsync() ?? item.AsBaseStorageFolder()?.GetBasicPropertiesAsync();
                var basicProps = ret != null ? await ret : null;
                props["System.ItemPathDisplay"] = item?.Path;
                props["System.DateCreated"] = basicProps?.ItemDate;
                props["System.DateModified"] = basicProps?.DateModified;
                return props;
            });
        }
    }
}
