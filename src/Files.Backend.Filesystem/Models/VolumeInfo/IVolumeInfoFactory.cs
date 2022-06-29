using Files.Backend.Filesystem.Models;
using System.Threading.Tasks;

namespace Files.Backend.Filesystem.Services
{
    public interface IVolumeInfoFactory
    {
        Task<VolumeInfo> BuildVolumeInfo(string driveName);
    }
}
