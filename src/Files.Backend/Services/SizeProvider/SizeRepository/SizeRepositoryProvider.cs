using CommunityToolkit.Mvvm.DependencyInjection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Files.Backend.Services.SizeProvider
{
    internal class SizeRepositoryProvider : ISizeRepositoryProvider
    {
        private SizeDatabase? database;

        public async Task<ISizeRepository> GetSizeRepository(string driveName, CancellationToken cancellationToken = default)
        {
            var volumeInfoFactory = Ioc.Default.GetService<IVolumeInfoFactory>();
            if (volumeInfoFactory is IVolumeInfoFactory factory)
            {
                var info = await factory.BuildVolumeInfo(driveName);
                if (!info.IsEmpty)
                {
                    var database = GetDatabase();
                    var collection = database.GetCollection(info.Guid);
                    return new LiteDbSizeRepository(collection);
                }
            }
            return new DictionarySizeRepository();
        }

        private SizeDatabase GetDatabase()
        {
            if (database is null)
            {
                string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "foldersizes.db");
                database = new SizeDatabase(path);
            }
            return database;
        }
    }
}
