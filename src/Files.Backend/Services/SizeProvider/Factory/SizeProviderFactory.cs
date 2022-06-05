using System.Threading.Tasks;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizeProviderFactory : ISizeProviderFactory
    {
        public async Task<ISizeProvider> CreateSizeProviderAsync(string driveName)
        {
            var provider = new SizeRepositoryProvider();
            var repository = await provider.GetSizeRepositoryAsync(driveName);
            return new PersistentSizeProvider(repository);
        }
    }
}
