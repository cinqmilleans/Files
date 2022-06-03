using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal interface ISizeProviderFactory
    {
        Task<ISizeProvider> CreateSizeProvider(string driveName);
    }
}
