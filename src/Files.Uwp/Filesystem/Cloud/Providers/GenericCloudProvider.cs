using Files.Shared.Cloud;
using Files.Uwp.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Files.Uwp.Filesystem.Cloud.Providers
{
    public class GenericCloudProvider : ICloudProviderDetector
    {
        public async IAsyncEnumerable<ICloudProvider> DetectAsync()
        {
            var connection = await AppServiceConnectionHelper.Instance;
            if (connection is not null)
            {
                var (status, response) = await connection.SendMessageForResponseAsync(new ValueSet
                {
                    ["Arguments"] = "DetectCloudDrives",
                });

                if (status is AppServiceResponseStatus.Success && response.ContainsKey("Drives"))
                {
                    var providers = JsonConvert.DeserializeObject<List<CloudProvider>>((string)response["Drives"]);
                    if (providers is not null)
                    {
                        foreach (var provider in providers)
                        {
                            yield return provider;
                        }
                    }
                }
            }
        }
    }
}