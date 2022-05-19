using Files.Backend.Enums;
using Files.Backend.Services;
using Files.Shared;
using Files.Uwp.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Media.Imaging;

namespace Files.Uwp.ServicesImplementation.Icon
{
    internal class SystemIconService : IIconService
    {
        private IReadOnlyDictionary<Icons, IconFileInfo> infos;

        public async Task<BitmapImage> GetImage(Icons icon)
        {
            var info = await GetIconResourceInfo(index);
            if (info is null)
            {
                return null;
            }
            return await iconInfo.IconDataBytes.ToBitmapAsync();
        }

    private static async Task<IList<IconFileInfo>> LoadAsync(string filePath, IList<int> indexes, int iconSize = 48)
    {
        var connection = await AppServiceConnectionHelper.Instance;
        if (connection is not null)
        {
            var value = new ValueSet
                {
                    { "Arguments", "GetSelectedIconsFromDLL" },
                    { "iconFile", filePath },
                    { "requestedIconSize", iconSize },
                    { "iconIndexes", JsonConvert.SerializeObject(indexes) }
                };
            var (status, response) = await connection.SendMessageForResponseAsync(value);
            if (status is AppServiceResponseStatus.Success)
            {
                var icons = JsonConvert.DeserializeObject<IList<IconFileInfo>>((string)response["IconInfos"]);
                if (icons is not null)
                {
                    foreach (IconFileInfo info in icons)
                    {
                        info.IconDataBytes = Convert.FromBase64String(info.IconData);
                    }
                }
                return icons;
            }
        }
        return null;
    }



    private static async Task<IconFileInfo> GetIconResourceInfo(int index)
        {
            var icons = await UIHelpers.IconResources;
        if (icons != null)
        {
            return icons.FirstOrDefault(x => x.Index == index);
        }
        return null;
    }*/

        private static int ToResourceIndex(Icons icon) => icon switch
        {
            Icons.Folder => 3,
            Icons.ThisPC => 109,
            Icons.QuickAccess => 1024,
            Icons.Desktop => 183,
            Icons.Libraries => 1023,
            Icons.Downloads => 184,
            Icons.Documents => 112,
            Icons.Pictures => 113,
            Icons.Music => 108,
            Icons.Videos => 189,
            Icons.RecycleBin => 55,
            Icons.WindowsDrive => 36,
            Icons.NetworkDrive => 25,
            Icons.GenericDiskDrive => 35,
            Icons.CloudDrive => 1040,
            Icons.OneDrive => 1043,
            _ => throw new ArgumentException(nameof(icon)),
        };
    }
}
