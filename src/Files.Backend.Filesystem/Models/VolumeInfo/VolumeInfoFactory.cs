using CommunityToolkit.Mvvm.DependencyInjection;
using Files.Backend.Filesystem.Models;
using Files.Shared.Services;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace Files.Backend.Filesystem.Services
{
    internal class VolumeInfoFactory : IVolumeInfoFactory
    {
        private readonly IFullTrustAsker asker = Ioc.Default.GetService<IFullTrustAsker>();

        public async Task<VolumeInfo> BuildVolumeInfo(string driveName)
        {
            string volumeId = await GetVolumeID(driveName);
            return new VolumeInfo(volumeId);
        }

        private async Task<string> GetVolumeID(string driveName)
        {
            if (asker is null)
            {
                return string.Empty;
            }

            var parameter = new ValueSet
            {
                ["Arguments"] = "VolumeID",
                ["DriveName"] = driveName,
            };

            var response = await asker.GetResponseAsync(parameter);
            if (response.IsSuccess)
            {
                return response.Get("VolumeID", string.Empty);
            }

            return string.Empty;
        }
    }
}
